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
        private List<GameObject> _affectedEnemies;
        private bool _isTimeDetonate;
        
        private SoundManager _soundManager;

        private void Start()
        {
            _affectedEnemies = new List<GameObject>();
            _soundManager = GameObject.FindGameObjectWithTag("GameController")
                .GetComponent<SoundManager>();
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

        private void Detonate()
        {   
            _affectedEnemies.ForEach(e =>
            {
                if (e.gameObject != null)
                    e.GetComponent<Enemy>().TakeDamage(damage, stunTime);   
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
