using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Helpers;
using TezosSDK.Beacon;
using TezosSDK.Helpers;
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
        
        private List<Nft> _userNfts;
        private Dictionary<string, object> _equipment;
        
        public Action<List<Nft>> TokensReceived;

        [SerializeField] private int maxTokenCount = 20;
        [SerializeField] private string contract = "KT1DTJEAte2SE1dTJNWS1qSck8pCmGpVpD6X";
        

        void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _equipment = new Dictionary<string, object>();
            _userNfts = new List<Nft>();
            
            Instance = this;

            TezosManager.Instance.Wallet.EventManager.AccountDisconnected += AccountDisconnected;
            TezosManager.Instance.MessageReceiver.AccountConnected += AccountConnected;
            
            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += ChangedActiveScene;

            /*
            *
            * Test case
            *     
            */
            // _userNfts = new List<Nft>
            // {
            //     new Nft 
            //     {
            //         Name = "Viper",
            //         Description = "This gun will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Viper.png",
            //         Type = Type.Gun
            //     },
            //     new Nft 
            //     {
            //         Name = "Claw",
            //         Description = "This gun will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Claw.png",
            //         Type = Type.Gun
            //     },
            //     new Nft 
            //     {
            //         Name = "Sealer",
            //         Description = "This SMG will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Sealer.png",
            //         Type = Type.Smg
            //     },
            //     new Nft 
            //     {
            //         Name = "Peacock",
            //         Description = "This SMG will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Peacock.png",
            //         Type = Type.Smg
            //     },
            //     new Nft 
            //     {
            //         Name = "Defender",
            //         Description = "This Shotgun will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Defender.png",
            //         Type = Type.Shotgun
            //     },
            //     new Nft 
            //     {
            //         Name = "DoomGuy",
            //         Description = "This Shotgun will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/DoomGuy.png",
            //         Type = Type.Shotgun
            //     },
            //     new Nft 
            //     {
            //         Name = "Roaster",
            //         Description = "This explosive will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Roaster.png",
            //         Type = Type.Explosive
            //     },
            //     new Nft 
            //     {
            //         Name = "Mines",
            //         Description = "This explosive will go up your ass",
            //         ThumbnailUri = "ipfs://QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Mines.png",
            //         Type = Type.Explosive
            //     },
            //     new Nft 
            //     {
            //         Name = "Tits Armor",
            //         Description = "These tits will give you pleasure",
            //         ThumbnailUri = "ipfs://QmdhY65Q8S8bXckHJAJvPMtZro1DyWK7xpQDaUsBotXUHQ/tits.png",
            //         Value = 2,
            //         Type = Type.Armor
            //     },
            //     new Nft 
            //     {
            //         Name = "Body Armor",
            //         Description = "These tits will give you pleasure",
            //         ThumbnailUri = "ipfs://QmdhY65Q8S8bXckHJAJvPMtZro1DyWK7xpQDaUsBotXUHQ/armor.png",
            //         Value = 4,
            //         Type = Type.Armor
            //     },
            //     new Nft 
            //     {
            //         Name = "Health",
            //         Description = "This module increases the initial health level by 5%.",
            //         Value = 5,
            //         ThumbnailUri = "ipfs://QmdhY65Q8S8bXckHJAJvPMtZro1DyWK7xpQDaUsBotXUHQ/health.png",
            //         Type = Type.Module
            //     },
            //     new Nft 
            //     {
            //         Name = "Damage",
            //         Description = "This module increases the initial damage level by 10%.",
            //         Value = 10,
            //         ThumbnailUri = "ipfs://QmdhY65Q8S8bXckHJAJvPMtZro1DyWK7xpQDaUsBotXUHQ/damage.png",
            //         Type = Type.Module
            //     },
            //     new Nft 
            //     {
            //         Name = "Speed",
            //         Description = "This module increases the initial movement speed level by 10%.",
            //         Value = 10,
            //         ThumbnailUri = "ipfs://QmdhY65Q8S8bXckHJAJvPMtZro1DyWK7xpQDaUsBotXUHQ/speed.png",
            //         Type = Type.Module
            //     }
            // };
            //
            // nftsReceived?.Invoke(_userNfts);
            /*
            *
            * Test case
            *     
            */
        }

        private void AccountConnected(AccountInfo account)
        {
            _connectedAddress = account.Address;
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("Address", null)))
            {
                PlayerPrefs.SetString("Address", _connectedAddress);

                // todo: sign server payload
                // Debug.Log(account.PublicKey);
                
                if (Camera.main)
                {
                    Camera.main.TryGetComponent<UiMenuManager>(out var manager);
                    if (manager == null) return;
                    manager.EnableGameMenu(_connectedAddress);
                }
            }
            LoadGameNfts();
        }
        
        private void AccountDisconnected(AccountInfo account)
        {
            PlayerPrefs.SetString("Address", null);

            if (!Camera.main) return;
            Camera.main.TryGetComponent<UiMenuManager>(out var manager);
            if (manager == null) return;
            manager.DisableGameMenu();
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

        private void ChangedActiveScene(Scene current, Scene next)
        {
            if (next.name != "Main") return;
            
            _userNfts.Clear();
            _equipment.Clear();
            LoadGameNfts();
        }

        public Dictionary<string, object> GetEquipment()
        {
            return _equipment;
        }

        public void SetEquipment(Dictionary<string, object> equipment)
        {
            _equipment = equipment;
        }

        private void OnDisable()
        {
            if (TezosManager.Instance == null) return;
            
            TezosManager.Instance.Wallet.EventManager.AccountDisconnected -= AccountDisconnected;
            TezosManager.Instance.Wallet.EventManager.AccountConnected -= AccountConnected;
        }
    }
}
