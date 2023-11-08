using Managers;
using UnityEngine;

namespace Weapons
{
    public class Shotgun : Weapon
    {
        [SerializeField] private int damage;
        [SerializeField] private float stunTime;
        [SerializeField] private int distance;
        [SerializeField] private int bulletsPerShot;
        [SerializeField] private float buckshotLifeTime;
        [SerializeField] private float inaccurancyDistanceBtwBuckshot;

        public GameObject damageEffect;
        public GameObject shootEffect;
        
        [SerializeField] private GameObject buckshot;

        void Awake()
        {
            /*
            * Test case
            */
            ammo = 30;
            /*
            * Test case
            */
            
            ReloadAmmo();
        }

        // Start is called before the first frame update
        void Start()
        {
            soundManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<SoundManager>();
        }
    
        private Vector3 GetShootingDirection()
        {
            var shotPosition = shotPoint.position;
            var targetPos = shotPosition + shotPoint.forward * distance;
            targetPos = new Vector3(
                targetPos.x + Random.Range(-inaccurancyDistanceBtwBuckshot, inaccurancyDistanceBtwBuckshot),
                shotPosition.y,
                targetPos.z + Random.Range(-inaccurancyDistanceBtwBuckshot, inaccurancyDistanceBtwBuckshot)
            );

            return targetPos - shotPosition;
        }

        private void DrawBuckshot(Vector3 end)
        {
            var lr = Instantiate(buckshot).GetComponent<LineRenderer>();
            lr.SetPositions(new Vector3[] {shotPoint.position, end});
            DestroyBuckshot(lr.gameObject);
        }

        private void DestroyBuckshot(GameObject go)
        {
            Destroy(go, buckshotLifeTime);
        }

        protected override void Shoot()
        {
            if (ammoQtyInMagazine <= 0 && weaponPurpose != WeaponPurpose.Enemy)
            {
                soundManager.TriggerFall();
                timeBtwShots = triggerFallInSec;
                return;
            }

            for (var i = 0; i < bulletsPerShot; i++)
            {
                if (Physics.Raycast(shotPoint.position, GetShootingDirection(), out var hit, distance))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Enemy"))
                        {
                            hit.collider.GetComponent<Enemy>().TakeDamage(damage, stunTime);
                            Instantiate(damageEffect, hit.transform.position, Quaternion.identity);
                        }
                        DrawBuckshot(hit.point);
                    }
                    else
                    {
                        DrawBuckshot(shotPoint.position + GetShootingDirection());
                    }
                }
                else
                {
                    DrawBuckshot(shotPoint.position + GetShootingDirection());   
                }
            }

            Instantiate(shootEffect, shotPoint.position, Quaternion.identity);
            
            base.Shoot();
        }
    }
}
