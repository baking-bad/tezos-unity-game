 using System;
 using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunType gunType;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shotPoint;
    
    [SerializeField] private float startTimeBtwShots;
    private float _timeBtwShots;
    private int _bulletsQty;
    
    public enum GunType
    {
        Default,
        Player,
        Enemy
    }
    
    public Action<int> bulletsQtyChanged;

    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeBtwShots <= 0)
        {
            if (Input.GetMouseButton(0) && gunType == GunType.Player && _bulletsQty > 0
                 || Input.GetMouseButton(0) && gunType == GunType.Default 
                || gunType == GunType.Enemy)
            {
                Shoot(); 
            }
        }
        else
        {
            _timeBtwShots -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        Instantiate(bullet, shotPoint.position, shotPoint.rotation);
        _soundManager.Shot();
        _timeBtwShots = startTimeBtwShots;
        
        if (gunType == GunType.Default) return;
        _bulletsQty--;
        bulletsQtyChanged?.Invoke(_bulletsQty);
    }

    public int GetBulletsQty()
    {
        return _bulletsQty;
    }

    public void ChangeBulletsQty(int qty)
    {
        _bulletsQty += qty;
        bulletsQtyChanged?.Invoke(_bulletsQty);
    }
}
