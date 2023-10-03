using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private SoundManager _soundManager;
    private PlayerController _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        _soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.health <= 0)
        {
            _soundManager.Lose();
            Stop();
            ShowResults();
        }
    }
    
    private void Stop()
    {
        Time.timeScale = 0;

        _player.enabled = false;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            e.GetComponent<Enemy>().enabled = false;
        }

        enabled = false;
    }

    private void ShowResults()
    {
        // TODO: show canvas with result
    }
}
