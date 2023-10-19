using System;
using Managers;
using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public WeaponPurpose weaponPurpose;
        public WeaponType weaponType;
        [SerializeField] protected GameObject bullet;
        [SerializeField] protected Transform shotPoint;
    
        [SerializeField] protected float startTimeBtwShots;
        protected float timeBtwShots;
        protected int bulletsQty;
    
        public enum WeaponPurpose
        {
            Player,
            Enemy
        }
    
        public enum WeaponType
        {
            Default,
            Gun,
            Shotgun,
            Mortar
        }
    
        public Action<int> bulletsQtyChanged;

        protected SoundManager soundManager;
    
        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<SoundManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (timeBtwShots <= 0)
            {
                if (Input.GetMouseButton(0) && weaponPurpose == WeaponPurpose.Player && bulletsQty > 0 
                    || Input.GetMouseButton(0) && weaponType == WeaponType.Default 
                    || weaponPurpose == WeaponPurpose.Enemy)
                {
                    Shoot(); 
                }
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
    
        protected virtual void Shoot()
        {
            if (weaponType != WeaponType.Shotgun)
                Instantiate(bullet, shotPoint.position, shotPoint.rotation);
        
            soundManager.Shot(weaponType);
            timeBtwShots = startTimeBtwShots;
        
            if (weaponType == WeaponType.Default || weaponPurpose == WeaponPurpose.Enemy) 
                return;
        
            bulletsQty--;
            bulletsQtyChanged?.Invoke(bulletsQty);
        }

        public int GetBulletsQty()
        {
            return bulletsQty;
        }

        public void ChangeBulletsQty(int qty)
        {
            bulletsQty += qty;
            bulletsQtyChanged?.Invoke(bulletsQty);
        }
    }
}
