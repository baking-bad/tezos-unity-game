using UnityEngine;

namespace Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float lifetime;
        [SerializeField] private int damage;
        [SerializeField] private float stunTime;
    
        [SerializeField] private LayerMask mask;

        [SerializeField] private bool enemyBullet;

        public GameObject damageEffect;
        public GameObject destroyEffect;

        private readonly float _distance = 1;
        // Start is called before the first frame update
        private void Start()
        {
            Invoke(nameof(DestroyBullet), lifetime);
        }

        // Update is called once per frame
        void Update()
        {
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _distance, mask);
        
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Enemy") && !enemyBullet)
                {
                    hit.collider.GetComponent<Enemy>().TakeDamage(damage, stunTime);
                    Instantiate(damageEffect, transform.position, Quaternion.identity);
                }
            
                if (hit.collider.CompareTag("Player") && enemyBullet)
                {
                    hit.collider.GetComponent<PlayerController>().ChangeHealth(-damage);
                    Instantiate(damageEffect, transform.position, Quaternion.identity);
                }

                DestroyBullet();
            }
        
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void DestroyBullet()
        {
            // Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
 