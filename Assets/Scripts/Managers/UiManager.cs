using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using WeaponType = Weapons.Weapon.WeaponType;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Slider healthBarValue;
        [SerializeField] private TMP_Text score;
        [SerializeField] private TMP_Text currentThreat;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TMP_Text weaponName;
        [SerializeField] private TMP_Text ammoQtyInMagazine;
        [SerializeField] private TMP_Text ammoQty;
        [SerializeField] private GameObject diedPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private TMP_Text resultScoreText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text waveThreatText;
        [SerializeField] private GameObject waveTip;
        [SerializeField] private GameObject bossTip;
        [SerializeField] private Sprite[] weaponSprites;
        [SerializeField] private Image shieldTimer;
        [SerializeField] private Image shieldTimerProgress;
        [SerializeField] private Text shieldTimerText;
        [SerializeField] private Image sprintTimerProgress;
        [SerializeField] private Image sprintTimerIcon;
        [SerializeField] private TMP_Text sprintTimerText;
        [SerializeField] private TMP_Text gameTimerText;
        
        private float _timer;
        private Animator _bossTipAnimator;
        private Animator _waveTipAnimator;
        
        private PlayerController _player;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            _waveTipAnimator = waveTip.GetComponent<Animator>();
            _bossTipAnimator = bossTip.GetComponent<Animator>();
            var levelManager = GetComponent<LevelManager>();
        
            levelManager.GameScoreUpdated += ScoreUpdated;
            levelManager.PlayerDied += ShowRestartPanel;
            levelManager.NewWaveHasBegun += NewWaveHasBegun;
            levelManager.BossSpawned += BossSpawned;
            levelManager.PauseGame += ShowPausePanel;
            levelManager.ResumeGame += HidePausePanel;
            _player.PlayerInitialized += SubscribeToPlayerEvents;
            _player.HealthChanged += PlayerHealthChanged;
            _player.WeaponSwitched += WeaponSwitched;
            _player.SprintCooldownContinues += SprintTimerChanged;
            _player.SprintCooldownStarted += SprintTimerStarted;
            _player.SprintCooldownEnded += SprintTimerEnded;

            score.text = "Score: " + levelManager.GetScore();
        }

        private void SubscribeToPlayerEvents()
        {
            _player.GetCurrentWeapon().AmmoQtyChanged += AmmoQtyChanged;
            _player.GetPlayerShield().ShieldTimerActivated += ShieldTimerActivated;
            _player.GetPlayerShield().ShieldTimerDeactivated += ShieldTimerDeactivated;
            _player.GetPlayerShield().ShieldTimerChanged += ShieldTimerChanged;
        }

        private void ScoreUpdated(int scr, int threat)
        {
            score.text = scr.ToString();
            currentThreat.text = "Current threat: " + threat;
        }

        private void PlayerHealthChanged(float maxHealth, float health, bool _)
        {
            healthBarValue.value = health / maxHealth;
        }

        private void WeaponSwitched(Weapon weapon)
        {
            foreach (var w in weaponSprites)
            {
                if (w.name != weapon.name) continue;
            
                weaponIcon.sprite = w;
                weaponName.text = w.name;

                var ammo = weapon.GetAmmo();

                ammoQtyInMagazine.text = ammo.Item1.ToString();
                
                ammoQty.text = weapon.weaponType == WeaponType.Gun
                    ? "/ Inf"
                    : "/ " + ammo.Item2;

                _player.GetCurrentWeapon().AmmoQtyChanged -= AmmoQtyChanged;
                weapon.AmmoQtyChanged += AmmoQtyChanged;
                
                break;
            }
        }

        private void AmmoQtyChanged(int ammoInMagazine, int ammo, WeaponType weaponType)
        {
            ammoQtyInMagazine.text = ammoInMagazine.ToString();
            ammoQty.text = weaponType == WeaponType.Gun
                ? "/ Inf"
                : "/ " + ammo;
        }

        private void FixedUpdate()
        {
            _timer += Time.deltaTime;
            gameTimerText.text = $"{(int) _timer / 60:00}:{(int) _timer % 60:00}";

        }

        private void NewWaveHasBegun(int wave, int waveThreat)
        {
            waveText.text = "Wave #" + wave;
            waveThreatText.text = "Wave threat: " + waveThreat;
            _waveTipAnimator.SetTrigger("Show");
        }
        
        private void BossSpawned(int wave, int waveThreat)
        {
            waveText.text = "Wave #" + wave;
            waveThreatText.text = "Wave threat: " + waveThreat;
            _bossTipAnimator.SetTrigger("Show");
        }

        private void ShowRestartPanel()
        {
            resultScoreText.text = "SCORE: " + score.text;
            diedPanel.SetActive(true);
        }

        private void ShowPausePanel()
        {
            pausePanel.SetActive(true);
        }
        
        private void HidePausePanel()
        {
            pausePanel.SetActive(false);
        }

        private void ShieldTimerActivated()
        {
            shieldTimer.gameObject.SetActive(true);
            shieldTimerProgress.fillAmount = 1;
        }
    
        private void ShieldTimerDeactivated()
        {
            shieldTimer.gameObject.SetActive(false);
            shieldTimerProgress.fillAmount = 1;
        }
    
        private void ShieldTimerChanged(float cooldown)
        {
            shieldTimerProgress.fillAmount -= 1 / cooldown * Time.deltaTime;
            shieldTimerText.text = (int)Math.Round(shieldTimerProgress.fillAmount * 100f) + "%";
        }
    
        private void SprintTimerChanged(float elapsedTime, float cooldown)
        {
            sprintTimerProgress.fillAmount += 1 / cooldown * Time.deltaTime;
            sprintTimerText.text = Math.Ceiling(cooldown - elapsedTime).ToString();
        }

        private void SprintTimerStarted()
        {
            sprintTimerText.gameObject.SetActive(true);
            var newColor = sprintTimerIcon.color;
            newColor.a = 0.5f;
            sprintTimerIcon.color = newColor;
            sprintTimerProgress.fillAmount = 0;
        }

        private void SprintTimerEnded()
        {
            sprintTimerProgress.fillAmount = 1;
            var newColor = sprintTimerIcon.color;
            newColor.a = 1;
            sprintTimerIcon.color = newColor;
            sprintTimerText.gameObject.SetActive(false);
        }

        protected void OnDisable()
        {
            var levelManager = GetComponent<LevelManager>();
            
            levelManager.GameScoreUpdated -= ScoreUpdated;
            levelManager.PlayerDied -= ShowRestartPanel;
            levelManager.NewWaveHasBegun -= NewWaveHasBegun;
            levelManager.BossSpawned -= BossSpawned;
            levelManager.PauseGame -= ShowPausePanel;
            levelManager.ResumeGame -= HidePausePanel;
            
            if (_player == null) return;

            _player.GetCurrentWeapon().AmmoQtyChanged -= AmmoQtyChanged;
            _player.PlayerInitialized -= SubscribeToPlayerEvents;
            _player.HealthChanged -= PlayerHealthChanged;
            _player.WeaponSwitched -= WeaponSwitched;
            _player.SprintCooldownContinues -= SprintTimerChanged;
            _player.SprintCooldownStarted -= SprintTimerStarted;
            _player.SprintCooldownEnded -= SprintTimerEnded;
        }
    }
}
