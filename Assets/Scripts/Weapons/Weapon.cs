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
        [SerializeField] public ParticleSystem fireEffect;
        [SerializeField] public ParticleSystem bulletShellEffect;
    
        [SerializeField] protected float fireRate;
        [SerializeField] protected int magazineCapacity;
        [SerializeField] protected float reloadTime;
        [SerializeField] public bool baseWeapon;
        [HideInInspector] public bool isUnlocked = true;
        
        protected float timeBtwShots;
        protected bool reloading;
        protected float timeBtwReloading;
        protected int ammoQtyInMagazine;
        protected int ammo;
        protected float triggerFallInSec = 1f;

        protected Animator animator;

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
    
        public Action<int, int, WeaponType> AmmoQtyChanged;

        protected SoundManager SoundManager;
        private LevelManager _levelManager;

        protected virtual void Awake()
        {
            animator = GetComponentInParent<Animator>();
            var gameController = GameObject.FindGameObjectWithTag("GameController");
            SoundManager = gameController.GetComponent<SoundManager>();
            _levelManager = gameController.GetComponent<LevelManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            ReloadAmmo();
            if (weaponPurpose == WeaponPurpose.Player) return;
            
            timeBtwShots = reloadTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (_levelManager.gameIsPaused) return;
            
            if (timeBtwShots <= 0)
            {
                if (Input.GetMouseButton(0) && 
                    weaponPurpose == WeaponPurpose.Player && 
                    !reloading)
                {
                    Shoot();
                }

                if (weaponPurpose == WeaponPurpose.Enemy)
                {
                    AttackStarted();
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
                SoundManager.Reload(this);
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
            AmmoQtyChanged?.Invoke(ammoQtyInMagazine, ammo, weaponType);
        }

        protected virtual void Shoot()
        {
            timeBtwShots = fireRate;
            
            if (ammoQtyInMagazine <= 0 && 
                weaponPurpose != WeaponPurpose.Enemy)
            {
                SoundManager.TriggerFall();
                timeBtwShots = triggerFallInSec;

                return;
            }

            AttackStarted();
            
            if (weaponType != WeaponType.Shotgun)
                Instantiate(bullet, shotPoint.position, shotPoint.rotation);

            if (fireEffect != null)
            {
                fireEffect.gameObject.SetActive(true);
                fireEffect.Play();
            }

            if (bulletShellEffect != null)
            {
                bulletShellEffect.gameObject.SetActive(true);
                bulletShellEffect.Play();
            }
            
            SoundManager.Shot(this);

            if (weaponPurpose == WeaponPurpose.Enemy) 
                return;
            
            _levelManager.GetGameResult().UpdateShotsFired();
        
            ammoQtyInMagazine--;
            AmmoQtyChanged?.Invoke(ammoQtyInMagazine, ammo, weaponType);
        }

        public (int, int) GetAmmo()
        {
            return (ammoQtyInMagazine, ammo);
        }

        public void ChangeAmmoQty(int qty)
        {
            ammo += qty;
            
            if (!gameObject.activeInHierarchy) return;
            
            AmmoQtyChanged?.Invoke(ammoQtyInMagazine, ammo, weaponType);;
        }

        /// <summary>
        /// Call with animation clip
        /// </summary>
        public void AttackEnded()
        {
            if (animator != null)
                animator.SetBool("isAttacking", false);
        }
        

        private void AttackStarted()
        {
            if (animator != null)
                animator.SetBool("isAttacking", true);
        }
    }
}