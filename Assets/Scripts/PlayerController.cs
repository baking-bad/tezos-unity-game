using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;
    
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
    }

    public void ChangeHealth(float healthValue)
    {
        health += healthValue; ;
        healthChanged.Invoke(health);
    }

    public float GetPlayerHealth()
    {
        return health;
    }
}
