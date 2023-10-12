using System;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private float cooldown;

    private bool _isActivated;
    private float _elapsedTime;

    public Action shieldTimerActivated;
    public Action shieldTimerDeactivated;
    public Action<float> shieldTimerChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        _elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActivated) return;
        
        _elapsedTime += Time.deltaTime;
        shieldTimerChanged?.Invoke(cooldown);
        
        if (!(_elapsedTime >= cooldown)) return;
        
        Deactivate();
    }

    public void Activate()
    {
        _isActivated = true;
        _elapsedTime = 0;
        shieldTimerActivated?.Invoke();
    }

    private void Deactivate()
    {
        _isActivated = false;
        _elapsedTime = 0f;
        gameObject.SetActive(false);
        shieldTimerDeactivated?.Invoke();
    }
}
