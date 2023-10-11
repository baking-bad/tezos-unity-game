using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float speed;
    [SerializeField] private bool canMeleeDamage;
    [SerializeField] private float meleeAttackRate;
    public float meleeDamage;
    
    private float _timeBtwAttack;
    private float _stopTime;
    private float _normalSpeed;

    public GameObject damageEffect;
    private PlayerController _player;
    
    private SoundManager _soundManager;
    
    private GameObject _killAward;
    public Action<Transform, GameObject> enemyKilled;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
        _normalSpeed = speed;
        _soundManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopTime <= 0)
        {
            speed = _normalSpeed;
        }
        else
        {
            speed = 0;
            _stopTime -= Time.deltaTime;
        }
        if (health <= 0)
        {
            _soundManager.Death();
            enemyKilled?.Invoke(transform, _killAward);
            Destroy(gameObject);
        }
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canMeleeDamage) return;
        
        if (other.CompareTag("Player"))
        {
            if (_timeBtwAttack <= 0)
            {
                OnEnemyAttack();
            }
            else
            {
                _timeBtwAttack -= Time.deltaTime;
            }
        }
    }

    private void OnEnemyAttack()
    {
        Instantiate(damageEffect, _player.transform.position, Quaternion.identity);
        _player.ChangeHealth((int)Math.Round(-meleeDamage));
        _timeBtwAttack = meleeAttackRate;
    }

    public void TakeDamage(int damage, float stunTime)
    {
        _stopTime = stunTime;
        health -= damage;
    }

    public void SetKillAward(GameObject award)
    {
        _killAward = award;
    }
}
