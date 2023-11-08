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
    
        [SerializeField] protected float fireRate;
        [SerializeField] protected int magazineCapacity;
        [SerializeField] protected float reloadTime;
        protected float timeBtwShots;
        protected bool reloading;
        protected float timeBtwReloading;
        protected int ammoQtyInMagazine;
        protected int ammo;
        protected float triggerFallInSec = 1f;

        public enum WeaponPurpose
        {
            Player,
            Enemy
        }
    
        public enum WeaponType
        {
            Gun,
            Shotgun,
            Explosive,
            Smg
        }
    
        public Action<int, int, WeaponType> ammoQtyChanged;

        protected SoundManager soundManager;
        

        // Start is called before the first frame update
        void Start()
        {
            
            /*
             * Test case
             */
            if (weaponType == WeaponType.Explosive)
            {
                ammo = 100;
            }
            /*
           * Test case
           */
            
            

            ReloadAmmo();
            soundManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<SoundManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (timeBtwShots <= 0)
            {
                if (Input.GetMouseButton(0) && weaponPurpose == WeaponPurpose.Player && !reloading
                    || weaponPurpose == WeaponPurpose.Enemy)
                {
                    Shoot(); 
                }
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.R) && ammoQtyInMagazine < magazineCapacity && ammo > 0 && !reloading 
                || ammoQtyInMagazine == 0 && ammo > 0 && !reloading && weaponPurpose != WeaponPurpose.Enemy)
            {
                reloading = true;
                soundManager.Reload(weaponType, name);
                timeBtwReloading = reloadTime;
            }
            
            if (reloading)
            {
                ReloadAmmo();
            }
        }

        protected void ReloadAmmo()
        {
            timeBtwReloading -= Time.deltaTime;
            
            if (!(timeBtwReloading <= 0)) return;

            if (ammoQtyInMagazine == magazineCapacity) return;
            
            var allAmmo = ammoQtyInMagazine + ammo;
            if (allAmmo >= magazineCapacity)
            {
                ammo = ammo - magazineCapacity + ammoQtyInMagazine; 
                ammoQtyInMagazine = magazineCapacity;
            }
            else
            {
                ammoQtyInMagazine = allAmmo;
                ammo = 0;
            }

            reloading = false;
            ammoQtyChanged?.Invoke(ammoQtyInMagazine, ammo, weaponType);
        }

        protected virtual void Shoot()
        {
            timeBtwShots = fireRate;
            
            if (ammoQtyInMagazine <= 0 && weaponPurpose != WeaponPurpose.Enemy)
            {
                soundManager.TriggerFall();
                timeBtwShots = triggerFallInSec;
                return;
            }
            
            if (weaponType != WeaponType.Shotgun)
                Instantiate(bullet, shotPoint.position, shotPoint.rotation);
        
            soundManager.Shot(weaponType, name);

            if (weaponPurpose == WeaponPurpose.Enemy) 
                return;
        
            ammoQtyInMagazine--;
            ammoQtyChanged?.Invoke(ammoQtyInMagazine, ammo, weaponType);
        }

        public (int, int) GetAmmo()
        {
            return (ammoQtyInMagazine, ammo);
        }

        public void ChangeAmmoQty(int qty)
        {
            ammo += qty;
            
            if (!gameObject.activeInHierarchy) return;
            
            ammoQtyChanged?.Invoke(ammoQtyInMagazine, ammo, weaponType);;
        }
    }
}
