using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Weapons;

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
    private bool _isAttacking;
    private bool _isKilled;
    private float _normalSpeed;

    private bool _isTheBoss;
    private int _bossIndex;
    
    private PlayerController _player;
    private Animator _animator;
    private Rigidbody _rb;
    private Weapon _weapon;
    
    private SoundManager _soundManager;
    private List<GameObject> _killAwards;
    
    public GameObject takeDamageEffect;
    public Action<Enemy, Transform, List<GameObject>> EnemyKilled;
    
    private int _deadBodyLifetimeInSec = 5;
    
    [SerializeField] private Collider enemyModelCollider;

    private void Awake()
    {
        _killAwards = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        TryGetComponent<Weapon>(out _weapon);
        _normalSpeed = speed;
        _soundManager = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !_isKilled)
        {
            _animator.SetBool("dead", true);
            _soundManager.Death();
            EnemyKilled?.Invoke(this, transform, _killAwards);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _isKilled = true;
            
            if (_weapon != null) _weapon.enabled = false;
            
            if (enemyModelCollider != null) enemyModelCollider.isTrigger = true;

             enabled = false;
        }

        if (_isAttacking)
        {
            if (_timeBtwAttack > 0)
            {
                _timeBtwAttack -= Time.deltaTime;
            }
            else
            {
                _isAttacking = false;
                _animator.SetBool("isAttacking", false);
            }
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
        if (_isKilled)
        {
            transform.Translate(Vector3.down * Time.fixedDeltaTime / 4, Space.World);
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _player.transform.position,
                speed * Time.fixedDeltaTime);
        
            transform.rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
        
            _animator.SetBool("isMoving", _rb.velocity != Vector3.zero);   
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canMeleeDamage || _isKilled) return;

        if (!other.CompareTag("Player")) return;
        
        if (_timeBtwAttack > 0) return;
        
        OnEnemyAttack();
    }

    /// <summary>
    /// Call with animation clip
    /// </summary>
    private void Die()
    {
        enabled = true;
        Invoke(nameof(DestroyDeadBody), _deadBodyLifetimeInSec);
    }
    
    private void DestroyDeadBody()
    {
        Destroy(gameObject);
    }

    private void OnEnemyAttack()
    {
        _player.ChangeHealth(-meleeDamage);
        _animator.SetBool("isAttacking", true);
        _timeBtwAttack = meleeAttackRate;
        _isAttacking = true;
    }

    public void TakeDamage(float damage, float stunTime)
    {
        Instantiate(takeDamageEffect, gameObject.transform.position, Quaternion.identity);
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