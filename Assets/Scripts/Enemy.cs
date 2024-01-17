using System;
using System.Collections.Generic;
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

    private bool _isTheBoss;
    private int _bossIndex;
    
    private PlayerController _player;
    private SoundManager _soundManager;
    private List<GameObject> _killAwards;
    
    public GameObject damageEffect;
    public Action<Enemy, Transform, List<GameObject>> enemyKilled;

    private void Awake()
    {
        _killAwards = new List<GameObject>();
    }

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
            enemyKilled?.Invoke(this, transform, _killAwards);
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

    public void AddKillAward(GameObject award)
    {
        _killAwards.Add(award);
    }

    public void AppointBoss(int bossIndex)
    {
        _isTheBoss = true;
        _bossIndex = bossIndex;
    }

    public bool IsTheBoss()
    {
        return _isTheBoss;
    }
    
    public int GetBossIndex()
    {
        return _bossIndex;
    }
}
