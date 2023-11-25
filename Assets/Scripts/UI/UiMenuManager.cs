using System.Collections;
using SlimUI.ModernMenu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
	public class UiMenuManager : MonoBehaviour 
	{
		private Animator _cameraAnimator;
		
        [Header("MENUS")]
        public GameObject mainMenu;
        public GameObject playMenu;
        public GameObject exitMenu;
        public GameObject walletMenu;

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

			SetThemeColors();
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

		public void ConnectWallet()
		{
			exitMenu.SetActive(false);
			playMenu.SetActive(false);
			walletMenu.SetActive(true);
		}

		public void PlayCampaign()
		{
			exitMenu.SetActive(false);
			walletMenu.SetActive(false);
			playMenu.SetActive(true);
		}
		
		public void PlayCampaignMobile()
		{
			exitMenu.SetActive(false);
			walletMenu.SetActive(false);
			playMenu.SetActive(true);
		}

		public void ReturnMenu()
		{
			playMenu.SetActive(false);
			walletMenu.SetActive(false);
			exitMenu.SetActive(false);
			mainMenu.SetActive(true);
		}

		public void LoadScene(string scene)
		{
			if(scene != "")
			{
				StartCoroutine(LoadAsynchronously(scene));
			}
		}

		private void DisablePlayCampaign()
		{
			playMenu.SetActive(false);
		}
		
		public void Position3()
		{
			DisablePlayCampaign();
			_cameraAnimator.SetFloat("Animate",-1);
		}

		public void Position2()
		{
			DisablePlayCampaign();
			_cameraAnimator.SetFloat("Animate",1);
		}

		public void Position1()
		{
			_cameraAnimator.SetFloat("Animate",0);
		}

		void DisablePanels()
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
			DisablePanels();
			panelGame.SetActive(true);
			lineGame.SetActive(true);
		}

		public void VideoPanel()
		{
			DisablePanels();
			panelVideo.SetActive(true);
			lineVideo.SetActive(true);
		}

		public void AreYouSure()
		{
			exitMenu.SetActive(true);
			playMenu.SetActive(false);
			walletMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void AreYouSureMobile()
		{
			exitMenu.SetActive(true);
			DisablePlayCampaign();
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
	}
}