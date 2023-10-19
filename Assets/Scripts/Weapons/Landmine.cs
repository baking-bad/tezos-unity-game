using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Landmine : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private float stunTime;
    
        public GameObject damageEffect;
        public GameObject destroyEffect;
        public float explosionDelay;
        private List<GameObject> _affectedEnemies;
        private bool _isTimeDetonate;

        private void Start()
        {
            _affectedEnemies = new List<GameObject>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _affectedEnemies.Add(other.gameObject);

                if (_isTimeDetonate) return;
            
                _isTimeDetonate = true;
                Invoke(nameof(Detonate), explosionDelay);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _affectedEnemies.Remove(other.gameObject);
            }   
        }

        private void DestroyMine()
        {
            // Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private void Detonate()
        {   
            _affectedEnemies.ForEach(e =>
            {
                if (e.gameObject != null)
                    e.GetComponent<Enemy>().TakeDamage(damage, stunTime);   
            });
        
            Instantiate(damageEffect, transform.position, Quaternion.identity);
            DestroyMine();   
        }
    }
}
