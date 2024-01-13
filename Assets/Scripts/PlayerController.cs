using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Managers;
using UnityEngine;
using Weapons;
using Type = Nft.NftType;
using WeaponType = Weapons.Weapon.WeaponType;

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
    private WeaponType _weaponType;
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
    private float _damageReflectionInPercent;

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
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _weaponType = _currentWeapon.weaponType;
        weaponSwitched?.Invoke(_currentWeapon);
        _normalSpeed = moveSpeed;
        _timeBtwSprints = sprintCooldown;
        _maxHealth = health;
        
        playerInitialized?.Invoke();
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

        foreach (var item in equipment)
        {
            if (item.Type is Type.Gun or Type.Shotgun or Type.Smg or Type.Explosive)
            {
                AddWeapon(item);   
            }
            else
            {
                UpdatePlayerSkills(item);
            }
        }

        EnableDefaultGun();
    }

    private void EnableDefaultGun()
    {
        var gunType = WeaponType.Gun.ToString();
        foreach (var weapon in _equippedWeapons
                     .Where(w => w.Key == gunType))
        {
            weapon.Value.SetActive(true);
            var weaponScript = weapon.Value.GetComponent<Weapon>();
            weaponScript.isUnlocked = false;
            _currentWeapon = weaponScript;
            _weaponType = _currentWeapon.weaponType;
            break;
        }   
    }

    private void UpdatePlayerSkills(Nft module)
    {
        if (module.Type is not (Type.Module or Type.Ability or Type.Armor) 
            || module.GameParameters == null) return;

        if (module.GameParameters.Exists(p => p.Name == "Health"))
        {
            var healthParam = module.GameParameters
                .First(p => p.Name == "Health");
            _maxHealth = healthParam.MeasureType == GameParameters.Type.Percent
                ? health + health * healthParam.Value / 100f
                : health + healthParam.Value;
            health = _maxHealth;
            healthChanged?.Invoke(_maxHealth, health, false);
        }
        
        if (module.GameParameters.Exists(p => p.Name == "Speed"))
        {
            var speedParam = module.GameParameters
                .First(p => p.Name == "Speed");
            moveSpeed = speedParam.MeasureType == GameParameters.Type.Percent
                ? moveSpeed + moveSpeed * speedParam.Value / 100f
                : moveSpeed + speedParam.Value;
        }
        
        if (module.GameParameters.Exists(p => p.Name == "Damage"))
        {
            _damageIncreaseInPercent = module.GameParameters
                .First(p => p.Name == "Damage")
                .Value;
        }
        
        if (module.GameParameters.Exists(p => p.Name == "Armor"))
        {
            _damageReflectionInPercent = module.GameParameters
                .First(p => p.Name == "Armor")
                .Value;
        }
    }

    private void AddWeapon(Nft item)
    {
        if (item.Type is not (Type.Gun or Type.Shotgun or Type.Smg or Type.Explosive)) return;
        
        var weapons = GetAllWeapons();
        foreach (var w in weapons)
        {
            if (item.Name != w.name) continue;

            if (item.Type.ToString() != w.GetComponent<Weapon>().weaponType.ToString()) continue;
            
            GetEquippedWeapons()[item.Type.ToString()] = w;
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
        
        var newValue = damaged
            ? health + healthValue - healthValue * _damageReflectionInPercent / 100f
            : health + healthValue;
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
                _weaponType = _currentWeapon.weaponType;
            }
            else
            {
                foreach (var t in _equippedWeapons)
                {
                    _weaponType = _weaponType.Next();
                    var nextWeapon = _equippedWeapons[_weaponType.ToString()].GetComponent<Weapon>();
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
