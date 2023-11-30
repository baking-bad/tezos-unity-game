using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Managers;
using UI;
using UnityEngine;
using Weapons;
using Type = Nft.NftType;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintCooldown;
    [SerializeField] private float sprintDuration;

    /// <summary>
    /// Represent equipped weapons
    /// </summary>
    /// <typeparam name="string">Key by weapon type</typeparam>
    /// <typeparam name="GameObject">Equipped weapon</typeparam>
    private Dictionary<string, GameObject> _equippedWeapons;
    
    [SerializeField] private GameObject[] allWeapons;
    [SerializeField] private GameObject shield;

    private float _maxHealth;
    private Weapon _currentWeapon;
    private Weapon.WeaponType _currentType;
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
    
    private float _damageIncreaseInPercent;

    public Action<float, float, bool> healthChanged;
    public Action<Weapon> weaponSwitched;
    public Action<float> sprintCooldownContinues;
    public Action sprintCooldownStarted;
    public Action sprintCooldownEnded;
    public Action playerInitialized;

    private void Awake()
    {
        _shieldScript = shield.GetComponent<Shield>();
        EquipPlayer();
        
        playerInitialized?.Invoke();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _currentType = _currentWeapon.weaponType;
        weaponSwitched?.Invoke(_currentWeapon);
        _normalSpeed = moveSpeed;
        _timeBtwSprints = sprintCooldown;
        _maxHealth = health;
    }

    private void EquipPlayer()
    {
        _equippedWeapons = new Dictionary<string, GameObject>();
        var baseWeapons = GetAllWeapons()
            .Where(w => w.GetComponent<Weapon>().baseWeapon);
        
        foreach (var weapon in baseWeapons)
        {
            var weaponType = weapon.GetComponent<Weapon>().weaponType.ToString();
            if (_equippedWeapons.ContainsKey(weaponType))
                return;
            
            _equippedWeapons.Add(weaponType, weapon);
        }
        
        var equipment = UserDataManager.Instance.GetEquipment();
        UpdateSkillsByEquipment(equipment);
        
        EnableDefaultGun();
    }

    private void EnableDefaultGun()
    {
        var gunType = Weapon.WeaponType.Gun.ToString();
        foreach (var weapon in _equippedWeapons
                     .Where(w => w.Key == gunType))
        {
            weapon.Value.SetActive(true);
            var weaponScript = weapon.Value.GetComponent<Weapon>();
            weaponScript.isUnlocked = false;
            _currentWeapon = weaponScript;
            _currentType = _currentWeapon.weaponType;
            break;
        }   
    }

    private void UpdateSkillsByEquipment(Dictionary<string, object> equipment)
    {
        
        foreach (var item in equipment)
        {
            if (item.Value is not NftInventoryItem nft) return;
            
            switch (item.Key)
            {
                case "Health":
                    _maxHealth = health + health * nft.value / 100f;
                    health = _maxHealth;
                    healthChanged?.Invoke(_maxHealth, health, false);
                    break;
                
                case "Speed":
                    var speedIncrease = moveSpeed * nft.value / 100f;
                    moveSpeed += speedIncrease;
                    break;
                
                case "Damage":
                    _damageIncreaseInPercent = nft.value;
                    break;
                
                case "Armor":
                    break;
                
                default:
                    AddWeapon(nft);
                    break;
            }
        }
    }

    private void AddWeapon(NftInventoryItem nft)
    {
        if (nft.type is not (Type.Gun or Type.Shotgun or Type.Smg or Type.Explosive)) return;
        
        var weapons = GetAllWeapons();
        foreach (var w in weapons)
        {
            if (nft.title != w.name) continue;

            if (nft.type.ToString() != w.GetComponent<Weapon>().weaponType.ToString()) continue;
            
            GetEquippedWeapons()[nft.type.ToString()] = w;
            break;
        }
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
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        //     SwitchWeaponByName("Walky");
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        //     SwitchWeaponByName("Viper");
        // if (Input.GetKeyDown(KeyCode.Alpha3))
        //     SwitchWeaponByName("Claw");
        // if (Input.GetKeyDown(KeyCode.Alpha4))
        //     SwitchWeaponByName("Tiny");
        // if (Input.GetKeyDown(KeyCode.Alpha5))
        //     SwitchWeaponByName("Defender");
        // if (Input.GetKeyDown(KeyCode.Alpha6))
        //     SwitchWeaponByName("DoomGuy");
        // if (Input.GetKeyDown(KeyCode.Alpha7))
        //     SwitchWeaponByName("Fuzzy");
        // if (Input.GetKeyDown(KeyCode.Alpha8))
        //     SwitchWeaponByName("Peacock");
        // if (Input.GetKeyDown(KeyCode.Alpha9))
        //     SwitchWeaponByName("Sealer");
        // if (Input.GetKeyDown(KeyCode.Alpha0))
        //     SwitchWeaponByName("Mines");
        // if (Input.GetKeyDown(KeyCode.Comma))
        //     SwitchWeaponByName("Roaster");
        // if (Input.GetKeyDown(KeyCode.Period))
        //     SwitchWeaponByName("Zoooka");
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

    public void SwitchWeapon(GameObject weapon = null, bool isTaken = false)
    {
        if (_equippedWeapons.Count(w => !w.Value.GetComponent<Weapon>().isUnlocked) <= 1)
            return;

        foreach (var w in _equippedWeapons
                     .Where(w => w.Value.activeInHierarchy))
        {
            w.Value.SetActive(false);
        
            if (isTaken && weapon != null)
            {
                weapon.SetActive(true);
                _currentWeapon = weapon.GetComponent<Weapon>();
                _currentType = _currentWeapon.weaponType;
            }
            else
            {
                foreach (var t in _equippedWeapons)
                {
                    _currentType = _currentType.Next();
                    var nextWeapon = _equippedWeapons[_currentType.ToString()].GetComponent<Weapon>();
                    if (nextWeapon.isUnlocked) continue;
                    
                    nextWeapon.gameObject.SetActive(true);
                    _currentWeapon = nextWeapon;
                    break;
                }
            }

            weaponSwitched?.Invoke(_currentWeapon);

            break;
        }
    }
    
    // public void SwitchWeaponByName(string weaponName)
    // {
    //     for (var i = 0; i < _unlockedWeapons.Count; i++)
    //     {
    //         if (!_unlockedWeapons[i].activeInHierarchy) continue;
    //         
    //         _unlockedWeapons[i].SetActive(false);
    //     
    //         var weapon = _unlockedWeapons
    //             .FirstOrDefault(w => w.name == weaponName);
    //         
    //         if (weapon == null) return;
    //     
    //         weapon.SetActive(true);
    //         _currentWeapon = weapon.GetComponent<Weapon>();
    //         
    //         weaponSwitched?.Invoke(_currentWeapon);
    //         break;
    //     }
    // }

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

    public Dictionary<string, GameObject> GetEquippedWeapons()
    {
        return _equippedWeapons;
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
