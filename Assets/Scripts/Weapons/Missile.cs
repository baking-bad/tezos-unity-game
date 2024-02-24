using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Missile : Bullet
    {
        private List<GameObject> _affectedEnemies;
        
        protected override void Start()
        {
            _affectedEnemies = new List<GameObject>();
            base.Start();
        }

        protected override void DestroyBullet()
        {
            _affectedEnemies.ForEach(e =>
            {
                if (e.gameObject == null) return;
                
                e.GetComponent<Enemy>().TakeDamage(damage, stunTime);
            });
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _affectedEnemies.Add(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _affectedEnemies.Remove(other.gameObject);
            }   
        }
    }
}
