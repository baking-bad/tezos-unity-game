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

        private readonly float _inaccurancyDistance = 5f;

        public GameObject damageEffect;
        public GameObject shootEffect;
    
        [Header("Laser for shot test")]
        [SerializeField] private GameObject laser;
    
        // private List<GameObject> _affectedEnemies;

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
        
            // _affectedEnemies = new List<GameObject>();
        }
    
        private Vector3 GetShootingDirection()
        {
            var shotPosition = shotPoint.position;
            var targetPos = shotPosition + shotPoint.forward * distance;
            targetPos = new Vector3(
                targetPos.x + Random.Range(-_inaccurancyDistance, _inaccurancyDistance),
                shotPosition.y,
                targetPos.z + Random.Range(-_inaccurancyDistance, _inaccurancyDistance)
            );

            var dir = targetPos - shotPosition;
        
            return dir.normalized;
        }

        void TestLaser(Vector3 end)
        {
            var lr = Instantiate(laser).GetComponent<LineRenderer>();
            lr.SetPositions(new Vector3[] {shotPoint.position, end});
        }

        protected override void Shoot()
        {
            for (var i = 0; i < bulletsPerShot; i++)
            {
                if (Physics.Raycast(shotPoint.position, GetShootingDirection(), out var hit, distance))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Enemy"))
                        {
                            // _affectedEnemies.Add(hit.collider.gameObject);
                            hit.collider.GetComponent<Enemy>().TakeDamage(damage, stunTime);
                            Instantiate(damageEffect, hit.transform.position, Quaternion.identity);
                        }
                        // TestLaser(hit.point);
                    }
                    else
                    {
                        // TestLaser(shotPoint.position + GetShootingDirection());
                    }
                }
                else
                {
                    // TestLaser(shotPoint.position + GetShootingDirection());
                }

            }
        
            // _affectedEnemies.ForEach(e =>
            // {
            //     if (e.gameObject != null)
            //     {
            //         Instantiate(damageEffect, e.transform.position, Quaternion.identity);
            //         e.GetComponent<Enemy>().TakeDamage(damage, stunTime);
            //     }
            // });

            Instantiate(shootEffect, shotPoint.position, Quaternion.identity);

            base.Shoot();
        }
    }
}
