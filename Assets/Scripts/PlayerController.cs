using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int health;

    [SerializeField] private List<GameObject> unlockedWeapons;
    [SerializeField] private GameObject[] allWeapons;
    [SerializeField] private GameObject shield;
    
    private Gun _currentWeapon;
    private Shield _shieldScript;
    private Vector3 _movement;
    private Ray _ray;
    private RaycastHit _hit;
    
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
        weaponSwitched?.Invoke(_currentWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        _movement = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical"));
        
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(_ray, out _hit))
        {
            transform.LookAt(new Vector3(_hit.point.x, transform.position.y, _hit.point.z));
        }
        
        transform.Translate(_movement * moveSpeed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchWeapon();
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
