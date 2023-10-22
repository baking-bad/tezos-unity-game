using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weapons;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintCooldown;
    [SerializeField] private float sprintDuration;

    [SerializeField] private List<GameObject> unlockedWeapons;
    [SerializeField] private GameObject[] allWeapons;
    [SerializeField] private GameObject shield;
    
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
    
    public Action<int, bool> healthChanged;
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

    public void ChangeHealth(int healthValue, bool damaged = true)
    {
        if (shield.activeInHierarchy && (!shield.activeInHierarchy || healthValue <= 0)) return;
        
        health += healthValue; ;
        healthChanged?.Invoke(health, damaged);
    }

    private void SwitchWeapon(bool isTaken = false)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            foreach (var w in allWeapons)
            {
                if (other.name != w.name) continue;
                
                if (!unlockedWeapons.Exists(go => go.name == other.name))
                {
                    unlockedWeapons.Add(w);
                    SwitchWeapon(isTaken: true);
                }
                w.GetComponent<Weapon>().ChangeAmmoQty(30); // todo: TEMP
                
                break;
            }
            Destroy(other.gameObject);
        }
        
        if (other.CompareTag("HP"))
        {
            ChangeHealth(30, false);
            Destroy(other.gameObject);
        }
        
        if (other.CompareTag("Shield"))
        {
            shield.SetActive(true);
            _shieldScript.Activate();
            Destroy(other.gameObject);
        }
    }
}
