using System.Globalization;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private float _playerHealth;
    private int _score;
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private TMP_Text scoreDisplay;
    
    
    private PlayerController _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();

        _score = 0;
        _playerHealth = _player.health;
        scoreDisplay.text = "Score: " + _score;
        healthDisplay.text = "HP: " + _playerHealth.ToString(CultureInfo.InvariantCulture);

        _player.healthChanged += PlayerHealthChanged;
    }

    public void Kill()
    {
        _score++;
        scoreDisplay.text = "Score: " + _score;
    }

    private void PlayerHealthChanged(float health)
    {
        _playerHealth = health;
        healthDisplay.text = "HP: " + _playerHealth.ToString(CultureInfo.InvariantCulture);
    }
}
