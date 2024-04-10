using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Missile : Bullet
    {
        private List<Enemy> _affectedEnemies;
        
        protected override void Start()
        {
            _affectedEnemies = new List<Enemy>();
            base.Start();
        }

        protected override void DestroyBullet()
        {
            _affectedEnemies.ForEach(e =>
            {
                if (e.gameObject == null) return;
                
                e.TakeDamage(damage, stunTime);
            });
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") && other.enabled)
            {
                other.TryGetComponent<Enemy>(out var enemy);

                if (enemy == null)
                {
                    enemy = other.GetComponentInParent<Enemy>();
                    if (enemy == null) return;
                }
                
                _affectedEnemies.Add(enemy);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy") && other.enabled)
            {
                other.TryGetComponent<Enemy>(out var enemy);

                if (enemy == null)
                {
                    enemy = other.GetComponentInParent<Enemy>();
                    if (enemy == null) return;
                }
                
                _affectedEnemies.Remove(enemy);
            }   
        }
    }
}
