using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
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
    
    private float _maxHealth;
    private float _timeBtwAttack;
    private float _stopTime;
    private bool _isStunned;
    private bool _isAttacking;
    private bool _isKilled;
    private bool _isHidingDeadBody;
    private float _normalSpeed;

    private bool _isTheBoss;
    private int _bossIndex;
    
    private PlayerController _player;
    private Animator _animator;
    private Rigidbody _rb;
    private Weapon _weapon;

    private LevelManager _levelManager;
    private SoundManager _soundManager;
    private List<GameObject> _killAwards;
    
    public GameObject takeDamageEffect;
    public Action<Enemy, Transform, List<GameObject>> EnemyKilled;
    
    private int _deadBodyLifetimeInSec = 5;
    
    [SerializeField] private Collider enemyModelCollider;
    [SerializeField] private GameObject healthBar;
    private Slider _hpSlider;

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
        var gameController = GameObject.FindGameObjectWithTag("GameController");
        _levelManager = gameController.GetComponent<LevelManager>();
        _soundManager = gameController.GetComponent<SoundManager>();
        _maxHealth = health;
        if (healthBar == null) return;
        _hpSlider = healthBar.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_levelManager.gameIsPaused) return;
        
        if (health <= 0 && !_isKilled)
        {
            _animator.SetBool("dead", true);
            _soundManager.Death();
            EnemyKilled?.Invoke(this, transform, _killAwards);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _isKilled = true;
            
            if (healthBar != null) healthBar.SetActive(false);
            
            if (_weapon != null) _weapon.enabled = false;
            
            if (enemyModelCollider != null) enemyModelCollider.isTrigger = true;
            _rb.useGravity = false;

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
        if (_levelManager.gameIsPaused) return;
        
        if (_isHidingDeadBody)
        {
            transform.Translate(Vector3.down * Time.fixedDeltaTime, Space.World);
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
        _isHidingDeadBody = true;
        enabled = true;
        Invoke(nameof(DestroyDeadBody), _deadBodyLifetimeInSec);
    }
    
    /// <summary>
    /// Call with animation clip
    /// </summary>
    public void AttackEnded()
    {
        _animator.SetBool("isAttacking", false);
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
        
        if (healthBar == null) return;

        _hpSlider.value = health / _maxHealth;
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