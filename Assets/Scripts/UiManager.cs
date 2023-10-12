using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text bulletsQty;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text enemyHp;
    [SerializeField] private TMP_Text enemyDamage;
    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private Image shieldTimer;
    [SerializeField] private Image sprintTimer;

    private PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        var levelManager = GetComponent<LevelManager>();
        
        levelManager.scoreUpdated += ScoreUpdated;
        levelManager.playerDied += ShowRestartPanel;
        levelManager.levelDifficultyIncreased += LevelDifficultyIncreased;
        _player.healthChanged += PlayerHealthChanged;
        _player.weaponSwitched += WeaponSwitched;
        _player.sprintCooldownContinues += SprintTimerChanged;
        _player.sprintCooldownEnded += SprintTimerEnded;
        _player.GetCurrentWeapon().bulletsQtyChanged += BulletsQtyChanged;
        _player.GetPlayerShield().shieldTimerActivated += ShieldTimerActivated;
        _player.GetPlayerShield().shieldTimerDeactivated += ShieldTimerDeactivated;
        _player.GetPlayerShield().shieldTimerChanged += ShieldTimerChanged;

        score.text = "Score: " + levelManager.GetScore();
        health.text = "HP: " + _player.GetPlayerHealth();
    }

    private void ScoreUpdated(int scr)
    {
        score.text = "Score: " + scr;
    }

    private void PlayerHealthChanged(int hlth, bool _)
    {
        health.text = "HP: " + hlth;
    }

    private void WeaponSwitched(Gun weapon)
    {
        foreach (var w in weaponSprites)
        {
            if (w.name != weapon.name) continue;
            
            weaponIcon.sprite = w;

            bulletsQty.text = weapon.gunType == Gun.GunType.Default 
                ? "Inf." 
                : weapon.GetBulletsQty().ToString();

            _player.GetCurrentWeapon().bulletsQtyChanged -= BulletsQtyChanged;
            weapon.bulletsQtyChanged += BulletsQtyChanged;
                
            break;
        }
    }

    private void BulletsQtyChanged(int qty)
    {
        bulletsQty.text = qty.ToString();
    }

    private void LevelDifficultyIncreased(float enemyHlth, float enemyDmg)
    {
        enemyHp.text = "Enemy HP: " + enemyHlth;
        enemyDamage.text = "Enemy Damage: " + enemyDmg;
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
        sprintTimer.fillAmount -= 1 / cooldown * Time.deltaTime;
    }

    private void SprintTimerEnded()
    {
        sprintTimer.fillAmount = 1;
    }
}
