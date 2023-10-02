using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private float distance;
    [SerializeField] private int damage;
    
    [SerializeField] private LayerMask mask;

    [SerializeField] private bool enemyBullet;

    public GameObject destroyEffect;

    // Start is called before the first frame update
    private void Start()
    {
        Invoke(nameof(DestroyBullet), lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance, mask);
        
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy") && !enemyBullet)
            {
                hit.collider.GetComponent<Enemy>().TakeDamage(damage);
            }
            
            if (hit.collider.CompareTag("Player") && enemyBullet)
            {
                hit.collider.GetComponent<PlayerController>().ChangeHealth(-damage);
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
 