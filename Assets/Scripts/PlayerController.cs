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

    public void ChangeHealth(float healthValue)
    {
        health += healthValue; ;
        healthChanged.Invoke(health);
    }

    private void SwitchWeapon(bool isTaked = false)
    {
        for (var i = 0; i < unlockedWeapons.Count; i++)
        {
            if (!unlockedWeapons[i].activeInHierarchy) continue;
            
            unlockedWeapons[i].SetActive(false);

            if (isTaked)
            {
                unlockedWeapons[^1].SetActive(true);
            }
            else
            {
                if (i >= unlockedWeapons.Count - 1)
                {
                    unlockedWeapons[0].SetActive(true);
                }
                else
                {
                    unlockedWeapons[i + 1].SetActive(true);
                }
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
            if (!unlockedWeapons.Exists(w => w.name == other.name))
            {
                foreach (var g in allWeapons)
                {
                    if (other.name == g.name)
                    {
                        unlockedWeapons.Add(g);
                    }
                }
                SwitchWeapon(isTaked: true);
            }
            Destroy(other.gameObject);
        }
    }
}
