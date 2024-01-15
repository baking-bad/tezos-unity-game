using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api;
using Beacon.Sdk.Beacon.Sign;
using Helpers;
using TezosSDK.Helpers.Coroutines;
using TezosSDK.Tezos;
using TezosSDK.Tezos.API.Models.Filters;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Type = Nft.NftType;

namespace Managers
{
    public class UserDataManager : MonoBehaviour
    {
        public static UserDataManager Instance;

        private string _connectedAddress;
        private string _pubKey;

        private List<Nft> _userNfts;
        private List<Nft> _equipment;
        
        public Action<List<Nft>> TokensReceived;

        [SerializeField] private int maxTokenCount = 20;
        [SerializeField] private string contract = "KT1DTJEAte2SE1dTJNWS1qSck8pCmGpVpD6X";
        [SerializeField] private string serverApiUrl = "https://static.turborouter.keenetic.pro/api";

        private GameApi _api;

        void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _equipment = new List<Nft>();
            _userNfts = new List<Nft>();
            
            Instance = this;

            _api = new GameApi(serverApiUrl);

            TezosManager.Instance.Wallet.EventManager.WalletDisconnected += WalletDisconnected;
            TezosManager.Instance.Wallet.EventManager.WalletConnected += WalletConnected;
            TezosManager.Instance.Wallet.EventManager.PayloadSigned += PayloadSigned;

            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += ChangedActiveScene;
        }

        private void PayloadSigned(SignResult payload)
        {
            var routine = _api.VerifyPayload(_pubKey, payload.Signature, verified =>
            {
                if (!verified) return;
                
                PlayerPrefs.SetString("Address", _connectedAddress);
                GetMenuManager()?.EnableGameMenu(_connectedAddress);
            });
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        private void WalletConnected(WalletInfo wallet)
        {
            _connectedAddress = wallet.Address;
            _pubKey = wallet.PublicKey;
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("Address", null)))
            {
                var routine = _api.GetPayload(_pubKey, payload =>
                {
                    TezosManager.Instance.Wallet.RequestSignPayload(SignPayloadType.micheline, payload);
                });
                CoroutineRunner.Instance.StartWrappedCoroutine(routine);
            }
            else
            {
                GetMenuManager()?.EnableGameMenu(_connectedAddress);
            }
            
            LoadGameNfts();
        }
        
        private void WalletDisconnected(WalletInfo wallet)
        {
            PlayerPrefs.SetString("Address", null);
            GetMenuManager()?.DisableGameMenu();
        }

        private void StartGame()
        {
            // StartCoroutine(HttpHelper.GetRequest($"{serverApiUrl}/game/start/?address={_connectedAddress}",
            //     res =>
            //     {
            //         Debug.Log(res);
            //     }));
        }

        private void LoadGameNfts()
        {
            CoroutineRunner.Instance.StartCoroutine(
                TezosManager.Instance.Tezos.API.GetTokensForOwner(tbs=> 
                    {
                        if (tbs == null) return;
                        
                        var userTokens = tbs.ToList();
                        if (userTokens.Count > 0)
                        {
                            var tokens = userTokens
                                .Where(t => t.TokenContract.Address == contract)
                                .ToList();

                            var options = new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            };
                            options.Converters.Add(new JsonStringEnumConverter());
                            options.Converters.Add(new NftConverter());
                            
                            foreach (var t in tokens)
                            {
                                try
                                {
                                    var nft = t.TokenMetadata.Deserialize<Nft>(options);
                                    
                                    if (nft == null ||
                                        nft.Type == Type.None ||
                                        nft.GameParameters == null) continue;

                                    _userNfts.Add(nft);
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Serialization error: " + e);
                                }
                            }
                            
                            TokensReceived?.Invoke(_userNfts);
                        }
                        else
                        {
                            Debug.Log($"{_connectedAddress} has no tokens");
                        }
                    }, 
                    owner: _connectedAddress,
                    withMetadata: true,
                    maxItems: maxTokenCount,
                    orderBy: new TokensForOwnerOrder.Default(0)));
        }

        private UiMenuManager GetMenuManager()
        {
            if (!Camera.main) return null;
            Camera.main.TryGetComponent<UiMenuManager>(out var manager);
            return manager;
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {

            if (next.name == "Game")
            {
                StartGame();
            }

            if (next.name == "Main")
            {
                _userNfts.Clear();
                _equipment.Clear();
                LoadGameNfts();
            }
        }

        public List<Nft> GetEquipment()
        {
            return _equipment;
        }

        public void SetEquipment(List<Nft> equipment)
        {
            _equipment = equipment;
        }

        private void OnDisable()
        {
            if (TezosManager.Instance == null) return;
            
            TezosManager.Instance.Wallet.EventManager.WalletDisconnected -= WalletDisconnected;
            TezosManager.Instance.Wallet.EventManager.WalletConnected -= WalletConnected;
            TezosManager.Instance.Wallet.EventManager.PayloadSigned -= PayloadSigned;
        }
    }
}
