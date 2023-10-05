using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private TMP_Text resultDisplay;
    [SerializeField] private TMP_Text enemyHpDisplay;
    [SerializeField] private TMP_Text enemyDamageDisplay;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        var levelManager = GetComponent<LevelManager>();
        
        levelManager.scoreUpdated += ScoreUpdated;
        levelManager.playerDied += ShowRestartPanel;
        levelManager.levelDifficultyIncreased += LevelDifficultyIncreased;
        player.healthChanged += PlayerHealthChanged;
        
        scoreDisplay.text = "Score: " + levelManager.GetScore();
        healthDisplay.text = "HP: " + player.GetPlayerHealth();
    }

    private void ScoreUpdated(int score)
    {
        scoreDisplay.text = "Score: " + score;
    }

    private void PlayerHealthChanged(float health)
    {
        healthDisplay.text = "HP: " + health;
    }
    
    private void LevelDifficultyIncreased(float enemyHealth, float enemyDamage)
    {
        enemyHpDisplay.text = "Enemy HP: " + enemyHealth;
        enemyDamageDisplay.text = "Enemy Damage: " + enemyDamage;
    }

    private void ShowRestartPanel()
    {
        resultDisplay.text = scoreDisplay.text;
        restartPanel.SetActive(true);
    }
}
