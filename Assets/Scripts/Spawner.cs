using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    public Transform[] spawnPoints;

    private float _timeBtwSpawn;
    public float startTimeBtwSpawn;
    public float decreaseTime;
    public float minTime;

    public float enemyHealth;
    public float increaseHealth;
    public float enemyDamage;
    public float increaseDamage;

    // Update is called once per frame
    void Update()
    {
        enemyDamage += Time.deltaTime * increaseDamage;
        enemyHealth += Time.deltaTime * increaseHealth;

        if (_timeBtwSpawn <= 0)
        {
            var randomEnemy = Random.Range(0, enemies.Length);
            var randomPoint = Random.Range(0, spawnPoints.Length);
            var enemyScript = Instantiate(enemies[randomEnemy], spawnPoints[randomPoint].position, Quaternion.identity).GetComponent<Enemy>();
            enemyScript.health = enemyHealth;
            enemyScript.meleeDamage = enemyDamage;
            
            
            _timeBtwSpawn = startTimeBtwSpawn;
            if (startTimeBtwSpawn > minTime)
            {
                startTimeBtwSpawn -= decreaseTime;
            }
        }
        else
        {
            _timeBtwSpawn -= Time.deltaTime;
        }
    }
}
