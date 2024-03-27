using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api;
using Api.Models;
using Beacon.Sdk.Beacon.Sign;
using Helpers;
using TezosSDK.Helpers.Coroutines;
using TezosSDK.Tezos;
using TezosSDK.Tezos.API;
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

        private List<Nft> _contractNfts;
        private List<Nft> _userNfts;
        private List<Nft> _equipment;
        private List<Reward> _rewards;
        private PlayerStats _playerStats;
        private GameSession _gameSession;

        public Action<List<Nft>> TokensReceived;
        public Action<List<Nft>> RewardsAndTokensLoaded;
        public Action GameStarted;

        [SerializeField] private int maxTokenCount = 20;
        [SerializeField] private string contract = "KT1HtDEdFLQ1m8soCZ7kA1ieMSLxbGSwCX5F";
        [SerializeField] private string serverApiUrl = "https://game.baking-bad.org/back/api";

        private GameApi _api;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _equipment = new List<Nft>();
            _userNfts = new List<Nft>();
            _contractNfts = new List<Nft>();

            Instance = this;

            _api = new GameApi(serverApiUrl);
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += ChangedActiveScene;
        }

        private void Start()
        {
            TezosManager.Instance.Wallet.EventManager.WalletDisconnected += WalletDisconnected;
            TezosManager.Instance.Wallet.EventManager.WalletConnected += WalletConnected;
            TezosManager.Instance.Wallet.EventManager.PayloadSigned += PayloadSigned;
            TezosManager.Instance.Wallet.EventManager.ContractCallCompleted += OperationCompleted;
            // todo: TezosManager.Instance.Wallet.EventManager.ContractCallFailed

            TezosManager.Instance.Tezos.TokenContract = new TokenContract(contract);
        }

        private void PayloadSigned(SignResult payload)
        {
            var routine = _api.VerifyPayload(_pubKey, payload.Signature, verified =>
            {
                if (!verified) return;

                PlayerPrefs.SetString("Address", _connectedAddress);
                GetMenuManager()?.EnableGameMenu();
                GetMenuManager()?.HideSignAwaitingBadge();
                StartCoroutine(LoadGameNfts());
                LoadPlayerStats();
            });
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        private void OperationCompleted(OperationResult operationResult)
        {
            GetMenuManager().ShowSuccessOperationHash(operationResult.TransactionHash);
        }

        private void WalletConnected(WalletInfo wallet)
        {
            _connectedAddress = wallet.Address;
            _pubKey = wallet.PublicKey;
            var cacheAddress = PlayerPrefs.GetString("Address", null);
            if (string.IsNullOrEmpty(cacheAddress)
                || _connectedAddress != cacheAddress)
            {
                var routine = _api.GetPayload(_pubKey,
                    payload =>
                    {
                        TezosManager.Instance.Wallet.RequestSignPayload(SignPayloadType.micheline, payload);
                        GetMenuManager()?.ShowSignAwaitingBadge();
                    });
                CoroutineRunner.Instance.StartWrappedCoroutine(routine);
            }
            else
            {
                GetMenuManager()?.EnableGameMenu();
                StartCoroutine(LoadGameNfts());
                LoadPlayerStats();
            }
        }

        private void WalletDisconnected(WalletInfo wallet)
        {
            PlayerPrefs.SetString("Address", null);
            GetMenuManager()?.DisableGameMenu();
            ResetData();
            TokensReceived?.Invoke(new List<Nft>());
            RewardsAndTokensLoaded?.Invoke(new List<Nft>());
            Debug.Log($"Wallet {wallet.Address} disconnected");
        }

        private void ResetData()
        {
            _connectedAddress = string.Empty;
            _pubKey = string.Empty;
            _contractNfts = new List<Nft>();
            _userNfts = new List<Nft>();
            _equipment = new List<Nft>();
            _rewards = new List<Reward>();
            _playerStats = new PlayerStats();
            _gameSession = new GameSession();
        }

        public void StartGame()
        {
            var routine = _api.CreateGameSession(_connectedAddress, session =>
            {
                _gameSession = session;
                GameStarted?.Invoke();
            });
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        public GameSession GetCurrentGameSession() => _gameSession;

        public void EndGame(GameResult gameResult)
        {
            var routine = _api.EndGameSession(gameResult);
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        public void PauseGame(string gameId)
        {
            var routine = _api.PauseGame(gameId);
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        public void ResumeGame(string gameId)
        {
            var routine = _api.ResumeGame(gameId);
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        public void KillBoss(
            string gameId,
            int boss)
        {
            var routine = _api.KillBoss(gameId, boss);
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }

        private IEnumerator LoadGameNfts()
        {
            if (string.IsNullOrEmpty(_connectedAddress)) yield break;

            var userTokensCoroutine = CoroutineRunner.Instance.StartCoroutine(
                TezosManager.Instance.Tezos.API.GetTokensForOwner(tbs =>
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

                            _userNfts.Clear();

                            foreach (var t in tokens)
                            {
                                try
                                {
                                    var nft = t.TokenMetadata.Deserialize<Nft>(options);

                                    if (nft == null ||
                                        nft.Type == Type.None ||
                                        nft.GameParameters == null) continue;

                                    var tokenCount = int.Parse(t.Balance);
                                    for (var i = 0; i < tokenCount; i++)
                                    {
                                        nft.TokenId = int.Parse(t.TokenId);
                                        _userNfts.Add(nft);
                                    }
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

            var contractTokensCoroutine = CoroutineRunner.Instance.StartCoroutine(
                TezosManager.Instance.Tezos.API.GetTokensForContract(tokens =>
                    {
                        Debug.Log(tokens);
                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        };
                        options.Converters.Add(new JsonStringEnumConverter());
                        options.Converters.Add(new NftConverter());

                        _contractNfts.Clear();

                        foreach (var t in tokens)
                        {
                            try
                            {
                                var nft = t.TokenMetadata.Deserialize<Nft>(options);

                                if (nft == null ||
                                    nft.Type == Type.None ||
                                    nft.GameParameters == null) continue;

                                nft.TokenId = int.Parse(t.TokenId);
                                _contractNfts.Add(nft);
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Serialization error: " + e);
                            }
                        }
                    },
                    contractAddress: contract,
                    withMetadata: true,
                    maxItems: maxTokenCount,
                    orderBy: new TokensForContractOrder.Default(0)));

            var rewardsCoroutine = CoroutineRunner.Instance.StartCoroutine(
                _api.GetRewardsList(
                    _connectedAddress,
                    rewards =>
                    {
                        var menuManager = GetMenuManager();
                        _rewards = new List<Reward>(rewards);

                        menuManager.SetRewardsAmount(
                            _rewards.Aggregate(0, (acc, reward) => acc + reward.Amount)
                        );
                    }
                )
            );

            yield return userTokensCoroutine;
            yield return contractTokensCoroutine;
            yield return rewardsCoroutine;

            var rewardNfts = GetRewardNfts().ToList();
            RewardsAndTokensLoaded?.Invoke(rewardNfts);
        }

        private void LoadPlayerStats()
        {
            if (string.IsNullOrEmpty(_connectedAddress)) return;

            var coroutine = _api.GetPlayerStats(_connectedAddress,
                stats => _playerStats = stats);

            CoroutineRunner.Instance.StartWrappedCoroutine(coroutine);
        }

        public void TransferToken(int tokenId, string address)
        {
            TezosManager.Instance.Tezos.TokenContract.Transfer(
                TransferCompleted,
                address,
                tokenId,
                1);

            var uiManager = GetMenuManager();
            uiManager.ShowTxAwaitingBadge();
        }

        private void TransferCompleted(string txHash)
        {
            StartCoroutine(LoadGameNfts());
            var uiManager = GetMenuManager();
            uiManager.HideTxAwaitingBadge();
        }

        public IEnumerable<Nft> GetRewardNfts()
        {
            var rewardTokenIds = _rewards
                .Select(r => r.TokenId)
                .ToArray();

            var rewardNftList = _contractNfts
                .Where(nft => rewardTokenIds.Contains(nft.TokenId))
                .ToList();

            foreach (var rewardNft in rewardNftList)
            {
                rewardNft.Amount = _rewards
                    .Find(r => r.TokenId == rewardNft.TokenId)
                    .Amount;
            }

            return rewardNftList;
        }

        // Called from JS side after captcha checked.
        public void ClaimReward(string captchaData)
        {
            var uiManager = GetMenuManager();
            uiManager.HideRewardsWindow();
            uiManager.ShowTokensAwaitingBadge();

            CoroutineRunner.Instance.StartCoroutine(
                _api.ClaimReward(
                    _connectedAddress,
                    captchaData,
                    claimRewardResponse =>
                    {
                        if (string.IsNullOrEmpty(claimRewardResponse.OperationHash)) return;
                        OperationCompleted(new OperationResult
                            {
                                TransactionHash = claimRewardResponse.OperationHash
                            }
                        );
                        StartCoroutine(LoadGameNfts());
                        uiManager.HideTokensAwaitingBadge();
                    }
                )
            );
        }

        private UiMenuManager GetMenuManager()
        {
            if (!Camera.main) return null;
            Camera.main.TryGetComponent<UiMenuManager>(out var manager);
            return manager;
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            if (next.name == "Main")
            {
                _userNfts.Clear();
                _equipment.Clear();
                _contractNfts.Clear();
                Time.timeScale = 1f;
                StartCoroutine(LoadGameNfts());
                GetMenuManager()?.EnableGameMenu();
                LoadPlayerStats();
            }
        }

        public List<Nft> GetEquipment() => _equipment;
        public PlayerStats GetPlayerStats() => _playerStats;

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
            TezosManager.Instance.Wallet.EventManager.ContractCallCompleted -= OperationCompleted;

            SceneManager.activeSceneChanged -= ChangedActiveScene;
        }
    }
}