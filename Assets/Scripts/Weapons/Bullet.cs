using UnityEngine;

namespace Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float lifetime;
        [SerializeField] protected int damage;
        [SerializeField] protected float stunTime;
    
        [SerializeField] protected LayerMask mask;

        [SerializeField] protected bool enemyBullet;

        public GameObject damageEffect;
        public GameObject destroyEffect;

        private readonly float _distance = 1f;
        // Start is called before the first frame update
        private void Start()
        {
            Invoke(nameof(DestroyBullet), lifetime);
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Physics.Raycast(transform.position, transform.forward, out var hit, _distance, mask);

            if (hit.collider == null) return;

            if (hit.collider.CompareTag("Enemy") && !enemyBullet)
            {
                hit.collider.GetComponent<Enemy>().TakeDamage(damage, stunTime);
                Instantiate(damageEffect, transform.position, Quaternion.identity);
                DestroyBullet();
                return;
            }
            
            if (hit.collider.CompareTag("Player") && enemyBullet)
            {
                hit.collider.GetComponent<PlayerController>().ChangeHealth(-damage);
                Instantiate(damageEffect, transform.position, Quaternion.identity);
                DestroyBullet();
                return;
            }
            
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            DestroyBullet();
        }

        protected virtual void DestroyBullet()
        {
            // Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
 