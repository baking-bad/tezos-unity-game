using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UiSettingsManager : MonoBehaviour 
	{
		public enum Platform
		{
			Desktop, 
			Mobile
		}
		
		public Platform platform;
		
		private GameObject _listener;
		private static float _musicVolume;
		private static float _sfxVolume;
		
		// toggle buttons
		[Header("MOBILE SETTINGS")]
		public GameObject mobileSfXtext;
		public GameObject mobileMusicText;
		public GameObject mobileShadowofftextLINE;
		public GameObject mobileShadowlowtextLINE;
		public GameObject mobileShadowhightextLINE;

		[Header("VIDEO SETTINGS")]
		public GameObject fullscreentext;
		public GameObject ambientocclusiontext;
		public GameObject shadowofftextLINE;
		public GameObject shadowlowtextLINE;
		public GameObject shadowhightextLINE;
		public GameObject aaofftextLINE;
		public GameObject aa2xtextLINE;
		public GameObject aa4xtextLINE;
		public GameObject aa8xtextLINE;
		public GameObject vsynctext;
		public GameObject motionblurtext;
		public GameObject texturelowtextLINE;
		public GameObject texturemedtextLINE;
		public GameObject texturehightextLINE;
		public GameObject cameraeffectstext; 

		[Header("GAME SETTINGS")]
		public GameObject showHudText;
		public GameObject musicSlider;
		public GameObject sfxSlider;

		public Action<float, float> soundVolumeChanged;
		public void  Start ()
		{
			if (Camera.main == null) return;
			
			_listener = Camera.main.gameObject;
			_musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
			_sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1);
			_listener.GetComponent<AudioSource>().volume = _musicVolume;
			
			musicSlider.GetComponent<Slider>().value = _musicVolume;
			sfxSlider.GetComponent<Slider>().value = _sfxVolume;

			// check full screen
			if (Screen.fullScreen)
			{
				fullscreentext.GetComponent<TMP_Text>().text = "on";
			}
			else if (!Screen.fullScreen)
			{
				fullscreentext.GetComponent<TMP_Text>().text = "off";
			}

			// check hud value
			if (PlayerPrefs.GetInt("ShowHud") == 0)
			{
				showHudText.GetComponent<TMP_Text>().text = "off";
			}
			else
			{
				showHudText.GetComponent<TMP_Text>().text = "on";
			}

			// check shadow distance/enabled
			if (platform == Platform.Desktop)
			{
				if (PlayerPrefs.GetInt("Shadows") == 0)
				{
					QualitySettings.shadowCascades = 0;
					QualitySettings.shadowDistance = 0;
					shadowofftextLINE.gameObject.SetActive(true);
					shadowlowtextLINE.gameObject.SetActive(false);
					shadowhightextLINE.gameObject.SetActive(false);
				}
				else if (PlayerPrefs.GetInt("Shadows") == 1)
				{
					QualitySettings.shadowCascades = 2;
					QualitySettings.shadowDistance = 75;
					shadowofftextLINE.gameObject.SetActive(false);
					shadowlowtextLINE.gameObject.SetActive(true);
					shadowhightextLINE.gameObject.SetActive(false);
				}
				else if (PlayerPrefs.GetInt("Shadows") == 2)
				{
					QualitySettings.shadowCascades = 4;
					QualitySettings.shadowDistance = 500;
					shadowofftextLINE.gameObject.SetActive(false);
					shadowlowtextLINE.gameObject.SetActive(false);
					shadowhightextLINE.gameObject.SetActive(true);
				}
			} 
			else if (platform == Platform.Mobile)
			{
				if (PlayerPrefs.GetInt("MobileShadows") == 0)
				{
					QualitySettings.shadowCascades = 0;
					QualitySettings.shadowDistance = 0;
					mobileShadowofftextLINE.gameObject.SetActive(true);
					mobileShadowlowtextLINE.gameObject.SetActive(false);
					mobileShadowhightextLINE.gameObject.SetActive(false);
				}
				else if (PlayerPrefs.GetInt("MobileShadows") == 1)
				{
					QualitySettings.shadowCascades = 2;
					QualitySettings.shadowDistance = 75;
					mobileShadowofftextLINE.gameObject.SetActive(false);
					mobileShadowlowtextLINE.gameObject.SetActive(true);
					mobileShadowhightextLINE.gameObject.SetActive(false);
				}
				else if (PlayerPrefs.GetInt("MobileShadows") == 2)
				{
					QualitySettings.shadowCascades = 4;
					QualitySettings.shadowDistance = 100;
					mobileShadowofftextLINE.gameObject.SetActive(false);
					mobileShadowlowtextLINE.gameObject.SetActive(false);
					mobileShadowhightextLINE.gameObject.SetActive(true);
				}
			}


			// check vsync
			if (QualitySettings.vSyncCount == 0)
			{
				vsynctext.GetComponent<TMP_Text>().text = "off";
			}
			else if (QualitySettings.vSyncCount == 1)
			{
				vsynctext.GetComponent<TMP_Text>().text = "on";
			}

			// check motion blur
			if (PlayerPrefs.GetInt("MotionBlur") == 0)
			{
				motionblurtext.GetComponent<TMP_Text>().text = "off";
			}
			else if (PlayerPrefs.GetInt("MotionBlur") == 1)
			{
				motionblurtext.GetComponent<TMP_Text>().text = "on";
			}

			// check ambient occlusion
			if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
			{
				ambientocclusiontext.GetComponent<TMP_Text>().text = "off";
			}
			else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
			{
				ambientocclusiontext.GetComponent<TMP_Text>().text = "on";
			}

			// check texture quality
			if (PlayerPrefs.GetInt("Textures") == 0)
			{
				QualitySettings.globalTextureMipmapLimit = 2;
				texturelowtextLINE.gameObject.SetActive(true);
				texturemedtextLINE.gameObject.SetActive(false);
				texturehightextLINE.gameObject.SetActive(false);
			}
			else if (PlayerPrefs.GetInt("Textures") == 1)
			{
				QualitySettings.globalTextureMipmapLimit = 1;
				texturelowtextLINE.gameObject.SetActive(false);
				texturemedtextLINE.gameObject.SetActive(true);
				texturehightextLINE.gameObject.SetActive(false);
			}
			else if (PlayerPrefs.GetInt("Textures") == 2)
			{
				QualitySettings.globalTextureMipmapLimit = 0;
				texturelowtextLINE.gameObject.SetActive(false);
				texturemedtextLINE.gameObject.SetActive(false);
				texturehightextLINE.gameObject.SetActive(true);
			}
		}
		

		public void FullScreen()
		{
			Screen.fullScreen = !Screen.fullScreen;

			if (Screen.fullScreen)
			{
				fullscreentext.GetComponent<TMP_Text>().text = "on";
			}
			else if (!Screen.fullScreen)
			{
				fullscreentext.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void MusicSliderChanged()
		{
			PlayerPrefs.SetFloat("MusicVolume", musicSlider.GetComponent<Slider>().value);
			UpdateVolume();
		}
		
		public void SfxSliderChanged()
		{
			PlayerPrefs.SetFloat("SfxVolume", sfxSlider.GetComponent<Slider>().value);
			UpdateVolume();
		}
		
		private void UpdateVolume()
		{
			_musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
			_sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1);
			GetComponent<AudioSource>().volume = _musicVolume;
			
			soundVolumeChanged?.Invoke(_musicVolume, _sfxVolume);
		}

		public void ShowHud()
		{
			if (PlayerPrefs.GetInt("ShowHud") == 0)
			{
				PlayerPrefs.SetInt("ShowHud", 1);
				showHudText.GetComponent<TMP_Text>().text = "on";
			}
			else if(PlayerPrefs.GetInt("ShowHud") == 1)
			{
				PlayerPrefs.SetInt("ShowHud", 0);
				showHudText.GetComponent<TMP_Text>().text = "off";
			}
		}

		// the playerprefs variable that is checked to enable mobile sfx while in game
		public void MobileSfxMute()
		{
			if (PlayerPrefs.GetInt("Mobile_MuteSfx") == 0)
			{
				PlayerPrefs.SetInt("Mobile_MuteSfx", 1);
				mobileSfXtext.GetComponent<TMP_Text>().text = "on";
			}
			else if (PlayerPrefs.GetInt("Mobile_MuteSfx") == 1)
			{
				PlayerPrefs.SetInt("Mobile_MuteSfx", 0);
				mobileSfXtext.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void MobileMusicMute()
		{
			if (PlayerPrefs.GetInt("Mobile_MuteMusic") == 0)
			{
				PlayerPrefs.SetInt("Mobile_MuteMusic", 1);
				mobileMusicText.GetComponent<TMP_Text>().text = "on";
			}
			else if (PlayerPrefs.GetInt("Mobile_MuteMusic") == 1)
			{
				PlayerPrefs.SetInt("Mobile_MuteMusic", 0);
				mobileMusicText.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void ShadowsOff()
		{
			PlayerPrefs.SetInt("Shadows", 0);
			QualitySettings.shadowCascades = 0;
			QualitySettings.shadowDistance = 0;
			shadowofftextLINE.gameObject.SetActive(true);
			shadowlowtextLINE.gameObject.SetActive(false);
			shadowhightextLINE.gameObject.SetActive(false);
		}

		public void ShadowsLow()
		{
			PlayerPrefs.SetInt("Shadows", 1);
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowDistance = 75;
			shadowofftextLINE.gameObject.SetActive(false);
			shadowlowtextLINE.gameObject.SetActive(true);
			shadowhightextLINE.gameObject.SetActive(false);
		}

		public void ShadowsHigh()
		{
			PlayerPrefs.SetInt("Shadows", 2);
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowDistance = 500;
			shadowofftextLINE.gameObject.SetActive(false);
			shadowlowtextLINE.gameObject.SetActive(false);
			shadowhightextLINE.gameObject.SetActive(true);
		}

		public void MobileShadowsOff()
		{
			PlayerPrefs.SetInt("MobileShadows", 0);
			QualitySettings.shadowCascades = 0;
			QualitySettings.shadowDistance = 0;
			mobileShadowofftextLINE.gameObject.SetActive(true);
			mobileShadowlowtextLINE.gameObject.SetActive(false);
			mobileShadowhightextLINE.gameObject.SetActive(false);
		}

		public void MobileShadowsLow()
		{
			PlayerPrefs.SetInt("MobileShadows", 1);
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowDistance = 75;
			mobileShadowofftextLINE.gameObject.SetActive(false);
			mobileShadowlowtextLINE.gameObject.SetActive(true);
			mobileShadowhightextLINE.gameObject.SetActive(false);
		}

		public void MobileShadowsHigh()
		{
			PlayerPrefs.SetInt("MobileShadows", 2);
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowDistance = 500;
			mobileShadowofftextLINE.gameObject.SetActive(false);
			mobileShadowlowtextLINE.gameObject.SetActive(false);
			mobileShadowhightextLINE.gameObject.SetActive(true);
		}

		public void Vsync()
		{
			if (QualitySettings.vSyncCount == 0)
			{
				QualitySettings.vSyncCount = 1;
				vsynctext.GetComponent<TMP_Text>().text = "on";
			}
			else if (QualitySettings.vSyncCount == 1)
			{
				QualitySettings.vSyncCount = 0;
				vsynctext.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void MotionBlur()
		{
			if (PlayerPrefs.GetInt("MotionBlur") == 0)
			{
				PlayerPrefs.SetInt("MotionBlur", 1);
				motionblurtext.GetComponent<TMP_Text>().text = "on";
			}
			else if (PlayerPrefs.GetInt("MotionBlur") == 1)
			{
				PlayerPrefs.SetInt("MotionBlur", 0);
				motionblurtext.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void AmbientOcclusion()
		{
			if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
			{
				PlayerPrefs.SetInt("AmbientOcclusion", 1);
				ambientocclusiontext.GetComponent<TMP_Text>().text = "on";
			}
			else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
			{
				PlayerPrefs.SetInt("AmbientOcclusion", 0);
				ambientocclusiontext.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void CameraEffects()
		{
			if (PlayerPrefs.GetInt("CameraEffects") == 0)
			{
				PlayerPrefs.SetInt("CameraEffects", 1);
				cameraeffectstext.GetComponent<TMP_Text>().text = "on";
			}
			else if (PlayerPrefs.GetInt("CameraEffects") == 1)
			{
				PlayerPrefs.SetInt("CameraEffects", 0);
				cameraeffectstext.GetComponent<TMP_Text>().text = "off";
			}
		}

		public void TexturesLow()
		{
			PlayerPrefs.SetInt("Textures", 0);
			QualitySettings.globalTextureMipmapLimit = 2;
			texturelowtextLINE.gameObject.SetActive(true);
			texturemedtextLINE.gameObject.SetActive(false);
			texturehightextLINE.gameObject.SetActive(false);
		}

		public void TexturesMed()
		{
			PlayerPrefs.SetInt("Textures", 1);
			QualitySettings.globalTextureMipmapLimit = 1;
			texturelowtextLINE.gameObject.SetActive(false);
			texturemedtextLINE.gameObject.SetActive(true);
			texturehightextLINE.gameObject.SetActive(false);
		}

		public void TexturesHigh()
		{
			PlayerPrefs.SetInt("Textures", 2);
			QualitySettings.globalTextureMipmapLimit = 0;
			texturelowtextLINE.gameObject.SetActive(false);
			texturemedtextLINE.gameObject.SetActive(false);
			texturehightextLINE.gameObject.SetActive(true);
		}
	}
}