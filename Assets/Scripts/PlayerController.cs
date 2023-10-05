using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;

    [SerializeField] private List<GameObject> unlockedWeapons;
    [SerializeField] private GameObject[] allWeapons;

    private Vector3 _movement;
    private Ray _ray;
    private RaycastHit _hit;
    
    public Action<float> healthChanged;

    // Update is called once per frame
    void Update()
    {
        _movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
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

    public void ChangeHealth(float healthValue)
    {
        health += healthValue; ;
        healthChanged.Invoke(health);
    }

    public void SwitchWeapon()
    {
        for (var i = 0; i < unlockedWeapons.Count; i++)
        {
            if (!unlockedWeapons[i].activeInHierarchy) continue;
            
            unlockedWeapons[i].SetActive(false);
            if (i != 0)
            {
                unlockedWeapons[i - 1].SetActive(true);
            }
            else
            {
                unlockedWeapons[^1].SetActive(true);
            }
            
            break;
        }
    }

    public float GetPlayerHealth()
    {
        return health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            for (var i = 0; i < allWeapons.Length; i++)
            {
                unlockedWeapons.Add(allWeapons[i]);
            }
            SwitchWeapon(); 
            Destroy(other.gameObject);
        }
    }
}
