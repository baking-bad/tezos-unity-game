using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using WeaponType = Weapons.Weapon.WeaponType;

namespace Managers
{
    public class HudManager : MonoBehaviour
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
        [SerializeField] private GameObject pauseButtons;
        [SerializeField] private GameObject pauseCountdown;
        [SerializeField] private GameObject sessionOverPopup;
        [SerializeField] private TMP_Text resultScoreText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text waveThreatText;
        [SerializeField] private GameObject waveTip;
        [SerializeField] private GameObject bossTip;
        [SerializeField] private GameObject nftTip;
        [SerializeField] private Sprite[] weaponSprites;
        [SerializeField] private Image shieldTimer;
        [SerializeField] private Image shieldTimerProgress;
        [SerializeField] private Text shieldTimerText;
        [SerializeField] private Image sprintTimerProgress;
        [SerializeField] private Image sprintTimerIcon;
        [SerializeField] private TMP_Text sprintTimerText;
        [SerializeField] private TMP_Text gameTimerText;
        [SerializeField] private GameObject bossHudPanel;
        [SerializeField] private GameObject bossHudPrefab;
        
        private float _timer;
        private Animator _bossTipAnimator;
        private Animator _waveTipAnimator;
        private Animator _nftTipAnimator;
        
        private PlayerController _player;
        private LevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<LevelManager>();
            _player = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            _waveTipAnimator = waveTip.GetComponent<Animator>();
            _bossTipAnimator = bossTip.GetComponent<Animator>();
            _nftTipAnimator = nftTip.GetComponent<Animator>();

            _levelManager.GameScoreUpdated += ScoreUpdated;
            _levelManager.PlayerDied += ShowRestartPanel;
            _levelManager.NewWaveHasBegun += NewWaveHasBegun;
            _levelManager.BossSpawned += BossSpawned;
            _levelManager.PauseGame += ShowPausePanel;
            _levelManager.ResumeGame += HidePausePanel;
            _levelManager.ResumeGameRequest += PauseCountdownStarted;
            _levelManager.DropNft += DropNft;
            _player.PlayerInitialized += SubscribeToPlayerEvents;
            _player.HealthChanged += PlayerHealthChanged;
            _player.WeaponSwitched += WeaponSwitched;
            _player.SprintCooldownContinues += SprintTimerChanged;
            _player.SprintCooldownStarted += SprintTimerStarted;
            _player.SprintCooldownEnded += SprintTimerEnded;
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
            if (_levelManager.gameIsPaused) return;
            
            _timer += Time.deltaTime;
            gameTimerText.text = $"{(int) _timer / 60:00}:{(int) _timer % 60:00}";

        }

        private void NewWaveHasBegun(int wave, int waveThreat)
        {
            waveText.text = "Wave #" + wave;
            waveThreatText.text = "Wave threat: " + waveThreat;
            _waveTipAnimator.SetTrigger("Show");
        }
        
        private void BossSpawned(Enemy boss, int wave, int waveThreat)
        {
            waveText.text = "Wave #" + wave;
            waveThreatText.text = "Wave threat: " + waveThreat;
            _bossTipAnimator.SetTrigger("Show");
            var bossHud = Instantiate(bossHudPrefab, bossHudPanel.transform);
            bossHud.GetComponent<BossHudResolver>().owner = boss;
        }
        
        private void DropNft()
        {
            _nftTipAnimator.SetTrigger("Show");
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
        
        private void HidePausePanel(bool resumed)
        {
            pausePanel.SetActive(false);
            pauseButtons.SetActive(true);
            pauseCountdown.SetActive(false);
            
            if (resumed) return;

            sessionOverPopup.SetActive(true);
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

        private void PauseCountdownStarted()
        {
            pauseButtons.SetActive(false);
            pauseCountdown.SetActive(true);
        }

        protected void OnDisable()
        {
            _levelManager.GameScoreUpdated -= ScoreUpdated;
            _levelManager.PlayerDied -= ShowRestartPanel;
            _levelManager.NewWaveHasBegun -= NewWaveHasBegun;
            _levelManager.BossSpawned -= BossSpawned;
            _levelManager.PauseGame -= ShowPausePanel;
            _levelManager.ResumeGame -= HidePausePanel;
            _levelManager.ResumeGameRequest -= PauseCountdownStarted;
            _levelManager.DropNft -= DropNft;
            
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
