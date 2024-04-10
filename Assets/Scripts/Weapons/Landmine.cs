using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Weapons
{
    public class Landmine : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private float stunTime;
        
        public GameObject explosiveEffect;
        public float explosionDelay;
        private List<Enemy> _affectedEnemies;
        private bool _isTimeDetonate;
        
        private SoundManager _soundManager;

        private void Start()
        {
            _affectedEnemies = new List<Enemy>();
            _soundManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<SoundManager>();
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

                if (_isTimeDetonate) return;
            
                _isTimeDetonate = true;
                Invoke(nameof(Detonate), explosionDelay);
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

        private void Detonate()
        {
            _affectedEnemies.ForEach(e =>
            {
                if (e.gameObject != null)
                    e.TakeDamage(damage, stunTime);   
            });
            
            _soundManager.MineDetanate();

            if (explosiveEffect != null)
            {
                Instantiate(explosiveEffect, gameObject.transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }
}
