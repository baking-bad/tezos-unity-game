using UnityEngine;

namespace Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float lifetime;
        [SerializeField] protected int damage;
        [SerializeField] protected float stunTime;
        // [SerializeField] protected LayerMask mask;
        [SerializeField] protected bool enemyBullet;

        public GameObject damageEffect;
        public GameObject destroyEffect;

        private Rigidbody _rb;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _rb = GetComponent<Rigidbody>();
            Invoke(nameof(DestroyBullet), lifetime);
        }

        private void FixedUpdate()
        {
            _rb.velocity = transform.forward * speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy") && !enemyBullet)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, stunTime);
                Instantiate(damageEffect, transform.position, Quaternion.identity);
                DestroyBullet();
                return;
            }
            
            if (collision.gameObject.CompareTag("Player") && enemyBullet)
            {
                collision.gameObject.GetComponent<PlayerController>().ChangeHealth(-damage);
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
 