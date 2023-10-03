using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    [SerializeField] private TMP_Text scoreDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        scoreDisplay.text = "Score: " + _score;
    }

    public void Kill()
    {
        _score++;
        scoreDisplay.text = "Score: " + _score;
    }
}
