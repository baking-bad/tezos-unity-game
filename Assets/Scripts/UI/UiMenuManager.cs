using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Managers;
using SlimUI.ModernMenu;
using TezosSDK.Helpers.Coroutines;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
	public class UiMenuManager : MonoBehaviour 
	{
		private Animator _cameraAnimator;

		[Header("MENUS")]
        public GameObject mainMenu;
        public GameObject playMenu;
        public GameObject exitMenu;
        public GameObject connectMenu;
        public GameObject walletMenu;
        
        [Header("BUTTONS")]
        public GameObject startGameButton;
        public GameObject connectWalletButton;
        public GameObject claimRewardButton;
        public GameObject changeWalletButton;
        public GameObject inventoryButton;

        public enum Theme {custom1, custom2, custom3};
        [Header("THEME SETTINGS")]
        public Theme theme;
        public ThemedUIData themeController;

        [Header("PANELS")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject panelVideo;
        [Tooltip("The UI Panel that holds the GAME window tab")]
        public GameObject panelGame;

        public GameObject panelInventory;
        public GameObject panelEffects;
        public GameObject panelStats;
        public GameObject rewardsWindow;
        public GameObject successOperationWindow;

        public GameObject tokensAwaitingBadge;
        public GameObject signAwaitingBadge;
        public GameObject txConfirmationAwaitingBadge;

        // highlights in settings screen
        [Header("SETTINGS SCREEN")]
        [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;
        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;

        // highlights in inventory screen
        [Header("INVENTORY SCREEN")]
        [Tooltip("Highlight Image for when INVENTORY Tab is selected in Inventory")]
        public GameObject lineInventory;
        [Tooltip("Highlight Image for when EFFECTS Tab is selected in Inventory")]
        public GameObject lineEffects;
        [Tooltip("Highlight Image for when STATS Tab is selected in Inventory")]
        public GameObject lineStats;

        [Header("LOADING SCREEN")]
		[Tooltip("If this is true, the loaded scene won't load until receiving user input")]
		public bool waitForInput = true;
        public GameObject loadingMenu;
		[Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
		public KeyCode userPromptKey;
		
		private GameObject _listener;
		private float _sfxVolume;
		
		[Header("SFX sounds")]
		public AudioClip hoverSound;
		public AudioClip sliderSound;
		public AudioClip swooshSound;
		
		[Header("UI Heroes")]
		public GameObject[] primaryHeroes;
		public GameObject[] secondaryHeroes;
		
		[Header("UI VFX")]
		public GameObject[] vfxs;

		public Action LoadingScreenShowed;
		
		[DllImport("__Internal")]
		private static extern void ShowCaptchaJS();
		
		[DllImport("__Internal")]
		private static extern void CopyToClipboardJS(string text);
		
		private void Awake()
		{
			SetThemeColors();
		}

		void Start()
		{
			if (Camera.main == null) return;

	        _listener = Camera.main.gameObject;
	        _listener.GetComponent<UiSettingsManager>().soundVolumeChanged += ChangeVolume;
	        
			_cameraAnimator = Camera.main
				.gameObject.transform
				.GetComponent<Animator>();

			playMenu.SetActive(false);
			exitMenu.SetActive(false);
			mainMenu.SetActive(true);
			
			var index = Random.Range(0, primaryHeroes.Length);
			primaryHeroes[index].SetActive(true);
			var indexVfx = Random.Range(0, vfxs.Length);
			vfxs[indexVfx].SetActive(true);
		}

		public void EnableGameMenu()
		{
			var address = PlayerPrefs.GetString("Address", null);
			if (string.IsNullOrEmpty(address)) return;
			connectWalletButton.SetActive(false);
			changeWalletButton.GetComponentInChildren<TMP_Text>().text = 
				address.Substring(0,5) + "..." + address.Substring(address.Length - 5, 5);
			changeWalletButton.SetActive(true);

			EnableButton(startGameButton, true);
			EnableButton(inventoryButton, true);
		}

		public void DisableGameMenu()
		{
			changeWalletButton.SetActive(false);
			claimRewardButton.SetActive(false);
			connectWalletButton.SetActive(true);

			EnableButton(startGameButton, false);
			EnableButton(inventoryButton, false);
				
			DisableAllMenu();
		}

		void SetThemeColors()
		{
			switch (theme)
			{
				case Theme.custom1:
					themeController.currentColor = themeController.custom1.graphic1;
					themeController.textColor = themeController.custom1.text1;
					break;
				case Theme.custom2:
					themeController.currentColor = themeController.custom2.graphic2;
					themeController.textColor = themeController.custom2.text2;
					break;
				case Theme.custom3:
					themeController.currentColor = themeController.custom3.graphic3;
					themeController.textColor = themeController.custom3.text3;
					break;
				default:
					Debug.Log("Invalid theme selected.");
					break;
			}
		}

		private void ChangeVolume(float _, float sfxVolume)
		{
			_sfxVolume = sfxVolume;
		}

		private void EnableButton(GameObject button, bool flag)
		{
			var imgComponent = button.GetComponent<Image>();
			imgComponent.color = flag
				? themeController.currentColor
				: Color.gray;
			imgComponent.raycastTarget = flag;
			button.GetComponent<Button>().interactable = flag;
			button.GetComponentInChildren<TMP_Text>().color = flag
				? themeController.textColor
				: Color.gray;
		}

		private void DisableAllMenu()
		{
			exitMenu.SetActive(false);
			playMenu.SetActive(false);
			walletMenu.SetActive(false);
			connectMenu.SetActive(false);
		}

		public void ConnectWallet()
		{
			DisableAllMenu();
			connectMenu.SetActive(true);
		}
		
		public void ChangeWallet()
		{
			DisableAllMenu();
			walletMenu.SetActive(true);
		}

		public void PlayCampaign()
		{
			DisableAllMenu();
			playMenu.SetActive(true);
		}

		public void ReturnMenu()
		{
			DisableAllMenu();
			mainMenu.SetActive(true);
		}

		public void LoadScene(string sceneName)
		{
			CoroutineRunner.Instance.StartCoroutine(LoadAsynchronously(sceneName));
		}

		public void Position3()
		{
			_cameraAnimator.SetFloat("Animate",-1);
		}

		public void Position2()
		{
			_cameraAnimator.SetFloat("Animate",1);
		}

		public void Position1()
		{
			_cameraAnimator.SetFloat("Animate",0);
		}

		public void ShowRewardsWindow()
		{
			Position3();
			rewardsWindow.SetActive(true);
		}

		public void HideRewardsWindow()
		{
			rewardsWindow.SetActive(false);
		}

		void DisableSettingsPanels()
		{
			panelVideo.SetActive(false);
			panelGame.SetActive(false);

			lineGame.SetActive(false);
			lineVideo.SetActive(false);
		}

		void DisableInventoryPanels()
		{
			panelStats.SetActive(false);
			panelInventory.SetActive(false);
			panelEffects.SetActive(false);
			
			lineInventory.SetActive(false);
			lineEffects.SetActive(false);
			lineStats.SetActive(false);
		}

		public void InventoryPanel()
		{
			DisableInventoryPanels();
			panelInventory.SetActive(true);
			lineInventory.SetActive(true);
		}
		
		public void EffectsPanel()
		{
			DisableInventoryPanels();
			panelEffects.SetActive(true);
			lineEffects.SetActive(true);
		}
		
		public void StatsPanel()
		{
			DisableInventoryPanels();
			panelStats.SetActive(true);
			lineStats.SetActive(true);
		}

		public void GamePanel()
		{
			DisableSettingsPanels();
			panelGame.SetActive(true);
			lineGame.SetActive(true);
		}

		public void VideoPanel()
		{
			DisableSettingsPanels();
			panelVideo.SetActive(true);
			lineVideo.SetActive(true);
		}

		public void AreYouSure()
		{
			DisableAllMenu();
			exitMenu.SetActive(true);
		}

		public void QuitGame()
		{
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}
		
		public void PlayHover()
		{
			AudioSource.PlayClipAtPoint(hoverSound, _listener.transform.position, _sfxVolume);
		}

		public void PlaySfxHover()
		{
			AudioSource.PlayClipAtPoint(sliderSound, _listener.transform.position, _sfxVolume);
		}

		public void PlaySwoosh()
		{
			AudioSource.PlayClipAtPoint(swooshSound, _listener.transform.position, _sfxVolume);
		}

		IEnumerator LoadAsynchronously(string sceneName)
		{
			var operation = SceneManager.LoadSceneAsync(sceneName);
			operation.allowSceneActivation = false;
			mainCanvas.SetActive(false);
			loadingMenu.SetActive(true);
			
			var index = Random.Range(0, secondaryHeroes.Length);
			secondaryHeroes[index].SetActive(true);

			LoadingScreenShowed?.Invoke();

			while (!operation.isDone)
			{
				var progress = Mathf.Clamp01(operation.progress / .95f);
				loadingBar.value = progress;

				if (operation.progress >= 0.9f && waitForInput)
				{
					loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
					loadingBar.value = 1;

					if (Input.GetKeyDown(userPromptKey))
					{
						UserDataManager.Instance.StartGame();
						operation.allowSceneActivation = true;
					}
                }
				else if(operation.progress >= 0.9f && !waitForInput)
				{
					operation.allowSceneActivation = true;
				}

				yield return null;
			}
		}

		protected void OnDisable()
		{
			_listener.GetComponent<UiSettingsManager>().soundVolumeChanged -= ChangeVolume;
		}

		public void SetRewardsAmount(int rewardsAmount)
		{
			if (rewardsAmount > 0)
			{
				claimRewardButton
						.GetComponentInChildren<TMP_Text>()
						.text = $"Claim {rewardsAmount} reward{(rewardsAmount > 1 ? "s" : "")}";
				claimRewardButton.SetActive(true);
			}
			else
			{
				claimRewardButton.SetActive(false);
			}
		}

		public void ShowSuccessOperationHash(string operationHash)
		{
			successOperationWindow.SetActive(true);
			successOperationWindow
				.GetComponentsInChildren<TMP_Text>()
				.First(c => c.name == "opHashText")
				.text = operationHash;
		}
		
		public void ShowSignAwaitingBadge() => signAwaitingBadge.SetActive(true);
		
		public void HideSignAwaitingBadge() => signAwaitingBadge.SetActive(false);

		public void HideSuccessOperationPopup() => successOperationWindow.SetActive(false);

		public void ShowTokensAwaitingBadge() => tokensAwaitingBadge.SetActive(true);
		
		public void HideTokensAwaitingBadge() => tokensAwaitingBadge.SetActive(false);
		public void ShowTxAwaitingBadge() => txConfirmationAwaitingBadge.SetActive(true);
		public void HideTxAwaitingBadge() => txConfirmationAwaitingBadge.SetActive(false);
		
		public void ShowCaptcha() => ShowCaptchaJS();

		public void CopyOperationHash()
		{
			var opHash = successOperationWindow
				.GetComponentsInChildren<TMP_Text>()
				.First(c => c.name == "opHashText")
				.text;
			CopyToClipboard(opHash);
		}

		private void CopyToClipboard(string text)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			CopyToClipboardJS(text);
#else
			GUIUtility.systemCopyBuffer = text;
#endif
		}
	}
}