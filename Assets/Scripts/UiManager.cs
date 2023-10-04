using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private TMP_Text resultDisplay;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        var levelManager = GetComponent<LevelManager>();
        
        levelManager.scoreUpdated += ScoreUpdated;
        levelManager.playerDied += ShowRestartPanel;
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

    private void ShowRestartPanel()
    {
        resultDisplay.text = scoreDisplay.text;
        restartPanel.SetActive(true);
    }
}
