using System;
using Managers;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float health;
    [SerializeField] private float speed;
    
    /// <summary>
    /// Each enemy prefab must have a unique threat value
    /// </summary>
    [SerializeField] public int threat;
    
    [SerializeField] private bool canMeleeDamage;
    [SerializeField] private float meleeAttackRate;
    [SerializeField] private float meleeDamage;
    
    private float _timeBtwAttack;
    private float _stopTime;
    private bool _isStunned;
    private float _normalSpeed;
    
    private PlayerController _player;
    private SoundManager _soundManager;
    private GameObject _killAward;
    
    public GameObject damageEffect;
    public Action<Enemy, Transform, GameObject> enemyKilled;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        _normalSpeed = speed;
        _soundManager = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            _soundManager.Death();
            enemyKilled?.Invoke(this, transform, _killAward);
            Destroy(gameObject);
        }

        if (!_isStunned) return;
        
        if (_stopTime <= 0)
        {
            speed = _normalSpeed;
            _isStunned = false;
        }
        else
        {
            speed = 0;
            _stopTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _player.transform.position,
            speed * Time.fixedDeltaTime);
        
        transform.rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canMeleeDamage) return;

        if (!other.CompareTag("Player")) return;
        
        if (_timeBtwAttack <= 0)
        {
            OnEnemyAttack();
        }
        else
        {
            _timeBtwAttack -= Time.deltaTime;
        }
    }

    private void OnEnemyAttack()
    {
        Instantiate(damageEffect, _player.transform.position, Quaternion.identity);
        _player.ChangeHealth(-meleeDamage);
        _timeBtwAttack = meleeAttackRate;
    }

    public void TakeDamage(float damage, float stunTime)
    {
        _isStunned = true;
        _stopTime = stunTime;

        var playerDamage = damage + damage * _player.GetPlayerDamageIncrease() / 100f;

        health -= playerDamage;
    }

    public void SetKillAward(GameObject award)
    {
        _killAward = award;
    }
}
