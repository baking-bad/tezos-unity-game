using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weapons;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintCooldown;
    [SerializeField] private float sprintDuration;

    [SerializeField] private List<GameObject> unlockedWeapons;
    [SerializeField] private GameObject[] allWeapons;
    [SerializeField] private GameObject shield;

    private float _maxHealth;
    private Weapon _currentWeapon;
    private Shield _shieldScript;
    private Vector3 _movement;
    private Vector3 _moveVector;
    private Rigidbody _rb;
    private Ray _ray;
    private RaycastHit _hit;
    private float _normalSpeed;
    private float _sprintTime;
    private float _timeBtwSprints;
    private bool _isSprinting;
    private bool _canSprint;

    private List<Nft> _userNfts;

    private float _healthIncreaseInPercent;
    private float _speedIncreaseInPercent;
    private float _damageIncreaseInPercent;

    public Action<float, float, bool> healthChanged;
    public Action<List<Nft>> nftsReceived;
    public Action<Weapon> weaponSwitched;
    public Action<float> sprintCooldownContinues;
    public Action sprintCooldownStarted;
    public Action sprintCooldownEnded;

    private void Awake()
    {
        _shieldScript = shield.GetComponent<Shield>();

        foreach (var weapon in unlockedWeapons
                     .Where(w => w.activeInHierarchy))
        {
            _currentWeapon = weapon.GetComponent<Weapon>();
            break;
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        weaponSwitched?.Invoke(_currentWeapon);
        _normalSpeed = moveSpeed;
        _timeBtwSprints = sprintCooldown;
        
        /*
        *
        * Test case
        *     
        */
        _userNfts = new List<Nft>();
        _userNfts.Add(new Nft 
        {
            Name = "Health",
            Description = "This module increases the initial health level by 5%.",
            Value = "5"
        });
        _userNfts.Add(new Nft 
        {
            Name = "Damage",
            Description = "This module increases the initial damage level by 10%.",
            Value = "10"
        });
        _userNfts.Add(new Nft 
        {
            Name = "Speed",
            Description = "This module increases the initial speed level by 15%.",
            Value = "15"
        });

        for (var i = 0; i < _userNfts.Count; i++)
        {
            switch (_userNfts[i].Name)
            {
                case "Health":
                    float.TryParse(_userNfts[i].Value, out var healthValue);
                    _healthIncreaseInPercent = healthValue;
                    break;
                
                case "Speed":
                    float.TryParse(_userNfts[i].Value, out var speedValue);
                    _speedIncreaseInPercent = speedValue;
                    break;
                
                case "Damage":
                    float.TryParse(_userNfts[i].Value, out var damageValue);
                    _damageIncreaseInPercent = damageValue;
                    break;
            }
        }
        
        nftsReceived?.Invoke(_userNfts);

        UpdateSkillsByNfts();
        /*
        *
        * Test case
        *     
        */


    }

    private void UpdateSkillsByNfts()
    {
        _maxHealth = health + health * _healthIncreaseInPercent / 100f;
        health = _maxHealth;
        healthChanged?.Invoke(_maxHealth, health, false);
        
        var speedIncrease = moveSpeed * _speedIncreaseInPercent / 100;
        moveSpeed += speedIncrease;
        
        // todo: increase all bullet damage
    }

    // Update is called once per frame
    void Update()
    {
        _moveVector = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical"));
        
        _moveVector.Normalize();
        _movement.Set(moveSpeed * _moveVector.x, 0f, moveSpeed * _moveVector.z);

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _canSprint)
        {
            _isSprinting = true;
            _canSprint = false;
            moveSpeed = sprintSpeed;
            _sprintTime = sprintDuration;
            _timeBtwSprints = 0;
            sprintCooldownStarted?.Invoke();
        }
        
        
        
        /*
        *
        * Test case
        *     
        */
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeaponByName("Walky");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeaponByName("Viper");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeaponByName("Claw");
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchWeaponByName("Tiny");
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchWeaponByName("Defender");
        if (Input.GetKeyDown(KeyCode.Alpha6))
            SwitchWeaponByName("DoomGuy");
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SwitchWeaponByName("Fuzzy");
        if (Input.GetKeyDown(KeyCode.Alpha8))
            SwitchWeaponByName("Peacock");
        if (Input.GetKeyDown(KeyCode.Alpha9))
            SwitchWeaponByName("Sealer");
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SwitchWeaponByName("Mines");
        if (Input.GetKeyDown(KeyCode.Comma))
            SwitchWeaponByName("Roaster");
        if (Input.GetKeyDown(KeyCode.Period))
            SwitchWeaponByName("Zoooka");
        /*
        *
        * Test case
        *     
        */
        
        

        if (_isSprinting)
        {
            if (_sprintTime > 0)
            {
                _sprintTime -= Time.deltaTime;
            }
            else
            {
                moveSpeed = _normalSpeed;
                _isSprinting = false;
            }
        }

        if (_canSprint) return;
        
        if (_timeBtwSprints < sprintCooldown)
        {
            sprintCooldownContinues?.Invoke(sprintCooldown);
            _timeBtwSprints += Time.deltaTime;
        }
        else
        {
            sprintCooldownEnded?.Invoke();
            _canSprint = true;
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _movement;
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(_ray, out _hit))
        {
            transform.LookAt(new Vector3(
                _hit.point.x,
                transform.position.y,
                _hit.point.z));
        }
    }

    public void ChangeHealth(float healthValue, bool damaged = true)
    {
        if (shield.activeInHierarchy && (!shield.activeInHierarchy || healthValue <= 0)) return;

        var newValue = health + healthValue;
        health = newValue > _maxHealth 
            ? _maxHealth
            : newValue;

        healthChanged?.Invoke(_maxHealth, health, damaged);
    }

    public void SwitchWeapon(bool isTaken = false)
    {
        for (var i = 0; i < unlockedWeapons.Count; i++)
        {
            if (!unlockedWeapons[i].activeInHierarchy) continue;
            
            unlockedWeapons[i].SetActive(false);

            if (isTaken)
            {
                unlockedWeapons[^1].SetActive(true);
                _currentWeapon = unlockedWeapons[^1].GetComponent<Weapon>();
            }
            else
            {
                if (i >= unlockedWeapons.Count - 1)
                {
                    unlockedWeapons[0].SetActive(true);
                    _currentWeapon = unlockedWeapons[0].GetComponent<Weapon>();
                }
                else
                {
                    unlockedWeapons[i + 1].SetActive(true);
                    _currentWeapon = unlockedWeapons[i + 1].GetComponent<Weapon>();
                }
            }
            weaponSwitched?.Invoke(_currentWeapon);
            break;
        }
    }
    
    public void SwitchWeaponByName(string weaponName)
    {
        for (var i = 0; i < unlockedWeapons.Count; i++)
        {
            if (!unlockedWeapons[i].activeInHierarchy) continue;
            
            unlockedWeapons[i].SetActive(false);

            var weapon = unlockedWeapons
                .FirstOrDefault(w => w.name == weaponName);
            
            if (weapon == null) return;

            weapon.SetActive(true);
            _currentWeapon = weapon.GetComponent<Weapon>();
            
            weaponSwitched?.Invoke(_currentWeapon);
            break;
        }
    }

    public float GetPlayerHealth()
    {
        return health;
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public Shield GetPlayerShield()
    {
        return _shieldScript;
    }

    public GameObject[] GetAllWeapons()
    {
        return allWeapons;
    }
    
    public List<GameObject> GetUnlockedWeapons()
    {
        return unlockedWeapons;
    }

    public float GetPlayerDamageIncrease()
    {
        return _damageIncreaseInPercent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Loot"))
        {
            other.GetComponent<Loot>().ApplyLoot(gameObject);
            Destroy(other.gameObject);
        }
    }
}
