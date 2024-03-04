using System;
using Managers;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private float cooldown;

    private bool _isActivated;
    private float _elapsedTime;

    public Action ShieldTimerActivated;
    public Action ShieldTimerDeactivated;
    public Action<float> ShieldTimerChanged;
    
    private LevelManager _levelManager;

    private void Awake()
    {
        _levelManager = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<LevelManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActivated) return;
        
        if (_levelManager.gameIsPaused) return;
        
        _elapsedTime += Time.deltaTime;
        ShieldTimerChanged?.Invoke(cooldown);
        
        if (!(_elapsedTime >= cooldown)) return;
        
        Deactivate();
    }

    public void Activate(float timeLimit)
    {
        cooldown = timeLimit;
        _isActivated = true;
        _elapsedTime = 0;
        ShieldTimerActivated?.Invoke();
    }

    private void Deactivate()
    {
        _isActivated = false;
        _elapsedTime = 0f;
        gameObject.SetActive(false);
        ShieldTimerDeactivated?.Invoke();
    }
}
