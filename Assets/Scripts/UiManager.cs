using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text scoreDisplay;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        var levelManager = GetComponent<LevelManager>();
        
        levelManager.scoreUpdated += ScoreUpdated;
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
}
