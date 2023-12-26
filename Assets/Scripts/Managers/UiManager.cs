using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using WeaponType = Weapons.Weapon.WeaponType;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Image healthBarValue;
        [SerializeField] private TMP_Text score;
        [SerializeField] private TMP_Text currentThreat;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TMP_Text weaponName;
        [SerializeField] private TMP_Text ammoQtyInMagazine;
        [SerializeField] private TMP_Text ammoQty;
        [SerializeField] private GameObject restartPanel;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text waveThreatText;
        [SerializeField] private TMP_Text waveAlertText;
        [SerializeField] private TMP_Text bossAlertText;
        [SerializeField] private Sprite[] weaponSprites;
        [SerializeField] private Image shieldTimer;
        [SerializeField] private Image sprintTimer;
        
        // [SerializeField] private Sprite[] valueSprites;
        // [SerializeField] private Image nftHealthIcon;
        // [SerializeField] private Image nftSpeedIcon;
        // [SerializeField] private Image nftDamageIcon;

        [SerializeField] private TMP_Text timerText;
        private float _timer;
        
        private Dictionary<string, Sprite> _spriteValues;
        
        private PlayerController _player;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            var levelManager = GetComponent<LevelManager>();
        
            levelManager.gameScoreUpdated += ScoreUpdated;
            levelManager.playerDied += ShowRestartPanel;
            levelManager.newWaveHasBegun += NewWaveHasBegun;
            levelManager.bossSpawned += BossSpawned;
            _player.playerInitialized += SubscribeToPlayerEvents;
            _player.healthChanged += PlayerHealthChanged;
            _player.weaponSwitched += WeaponSwitched;
            _player.sprintCooldownContinues += SprintTimerChanged;
            _player.sprintCooldownStarted += SprintTimerStarted;
            _player.sprintCooldownEnded += SprintTimerEnded;
            
            score.text = "Score: " + levelManager.GetScore();
        }
        
        void Start()
        {
            // InitSpriteValues();
        }

        private void SubscribeToPlayerEvents()
        {
            _player.GetCurrentWeapon().ammoQtyChanged += AmmoQtyChanged;
            _player.GetPlayerShield().shieldTimerActivated += ShieldTimerActivated;
            _player.GetPlayerShield().shieldTimerDeactivated += ShieldTimerDeactivated;
            _player.GetPlayerShield().shieldTimerChanged += ShieldTimerChanged;
        }

        // private void InitSpriteValues()
        // {
        //     _spriteValues = new Dictionary<string, Sprite>();
        //
        //     foreach (var s in valueSprites)
        //     {
        //         _spriteValues.Add(
        //             s.name,
        //             s);
        //     }
        // }

        // private void DrawUserNfts(List<Nft> nfts)
        // {
        //     try
        //     {
        //         foreach (var t in nfts)
        //         {
        //             switch (t.Name)
        //             {
        //                 case "Health":
        //                     nftHealthIcon.sprite = _spriteValues[t.Value.ToString()];
        //                     break;
        //                 case "Speed":
        //                     nftSpeedIcon.sprite = _spriteValues[t.Value.ToString()];
        //                     break;
        //                 case "Damage":
        //                     nftDamageIcon.sprite = _spriteValues[t.Value.ToString()];
        //                     break;
        //             }
        //         }
        //     }
        //     catch (Exception)
        //     {
        //         Console.WriteLine("Invalid dictionary key");
        //     }
        // }

        private void ScoreUpdated(int scr, int threat)
        {
            score.text = "Score: " + scr;
            currentThreat.text = "Current threat: " + threat;
        }

        private void PlayerHealthChanged(float maxHealth, float health, bool _)
        {
            healthBarValue.fillAmount = health / maxHealth;
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
                    ? "Inf"
                    : ammo.Item2.ToString();

                _player.GetCurrentWeapon().ammoQtyChanged -= AmmoQtyChanged;
                weapon.ammoQtyChanged += AmmoQtyChanged;
                
                break;
            }
        }

        private void AmmoQtyChanged(int ammoInMagazine, int ammo, WeaponType weaponType)
        {
            ammoQtyInMagazine.text = ammoInMagazine.ToString();
            ammoQty.text = weaponType == WeaponType.Gun
                ? "Inf"
                : ammo.ToString();
        }

        private void FixedUpdate()
        {
            _timer += Time.deltaTime;
            timerText.text = $"{(int) _timer / 60:00}:{(int) _timer % 60:00}";

        }

        private void NewWaveHasBegun(int wave, int waveThreat)
        {
            waveText.text = "Wave #" + wave;
            waveThreatText.text = "Wave threat: " + waveThreat;
            waveAlertText.gameObject.SetActive(true);
        }
        
        private void BossSpawned(int wave, int waveThreat)
        {
            waveText.text = "Wave #" + wave;
            waveThreatText.text = "Wave threat: " + waveThreat;
            bossAlertText.gameObject.SetActive(true);
        }

        private void ShowRestartPanel()
        {
            resultText.text = score.text;
            restartPanel.SetActive(true);
        }

        private void ShieldTimerActivated()
        {
            shieldTimer.gameObject.SetActive(true);
            shieldTimer.fillAmount = 1;
        }
    
        private void ShieldTimerDeactivated()
        {
            shieldTimer.gameObject.SetActive(false);
            shieldTimer.fillAmount = 1;
        }
    
        private void ShieldTimerChanged(float cooldown)
        {
            shieldTimer.fillAmount -= 1 / cooldown * Time.deltaTime;
        }
    
        private void SprintTimerChanged(float cooldown)
        {
            sprintTimer.fillAmount += 1 / cooldown * Time.deltaTime;
        }

        private void SprintTimerStarted()
        {
            sprintTimer.fillAmount = 0;
        }

        private void SprintTimerEnded()
        {
            sprintTimer.fillAmount = 1;
        }
    }
}
