using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    private Gun _currentWeapon;
    private Shield _shieldScript;
    private Vector3 _movement;
    private Vector3 _moveVector;
    private Rigidbody _rb;
    private Ray _ray;
    private RaycastHit _hit;
    private float _normalSpeed;
    private float _sprintTime;
    private float _timeBtwSprints;
    
    public Action<int, bool> healthChanged;
    public Action<Gun> weaponSwitched;

    private void Awake()
    {
        _shieldScript = shield.GetComponent<Shield>();

        foreach (var weapon in unlockedWeapons
                     .Where(w => w.activeInHierarchy))
        {
            _currentWeapon = weapon.GetComponent<Gun>();
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

        if (Input.GetKeyDown(KeyCode.Space) && _timeBtwSprints >= sprintCooldown)
        {
            moveSpeed = sprintSpeed;
            _sprintTime = sprintDuration;
            _timeBtwSprints = 0;
        }
        
        if (_sprintTime > 0)
        {
            _sprintTime -= Time.deltaTime;
        }
        else
        {
            moveSpeed = _normalSpeed;
        }

        if (_timeBtwSprints < sprintCooldown)
        {
            _timeBtwSprints += Time.deltaTime;
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
                _currentWeapon = unlockedWeapons[^1].GetComponent<Gun>();
            }
            else
            {
                if (i >= unlockedWeapons.Count - 1)
                {
                    unlockedWeapons[0].SetActive(true);
                    _currentWeapon = unlockedWeapons[0].GetComponent<Gun>();
                }
                else
                {
                    unlockedWeapons[i + 1].SetActive(true);
                    _currentWeapon = unlockedWeapons[i + 1].GetComponent<Gun>();
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

    public Gun GetCurrentWeapon()
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
                w.GetComponent<Gun>().ChangeBulletsQty(30); // todo: TEMP
                
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
