using UnityEngine;

public class Landmine : MonoBehaviour
{
    [SerializeField] private int damage;
    
    public GameObject damageEffect;
    public GameObject destroyEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            Instantiate(damageEffect, transform.position, Quaternion.identity);
            DestroyBullet();
        }
    }
    
    private void DestroyBullet()
    {
        // Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
