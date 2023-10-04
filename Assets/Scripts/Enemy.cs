using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float meleeDamage;
    public float speed;
    [SerializeField] private float startStopTime;
    [SerializeField] private float startTimeBtwAttack;
    
    private float _timeBtwAttack;
    private float _stopTime;
    private float _normalSpeed;

    public GameObject damageEffect;
    private PlayerController _player;
    
    private SoundManager _soundManager;
    
    public Action enemyKilled;


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
            enemyKilled.Invoke();
            Destroy(gameObject);
        }
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
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
        _player.ChangeHealth(-meleeDamage);
        _timeBtwAttack = startTimeBtwAttack;
    }

    public void TakeDamage(int damage)
    {
        _stopTime = startStopTime;
        health -= damage;
    }
}
