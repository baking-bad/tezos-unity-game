using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    /*--------------------------------------------------
    
    
    !!!! WARNING !!!!
    
    Each enemy prefab must have a unique threat value
    
    
    ------------------------------------------------------*/
    public class LevelManager : MonoBehaviour
    {
        public GameObject[] enemies;
        public GameObject[] bosses;
        public Transform[] spawnPoints;

        [Header("LOOT:")] [SerializeField] private GameObject[] supplyItems;
        [SerializeField] private GameObject[] weapons;
        [SerializeField] private int lootRate;

        [Header("WAVES PARAMS:")] 
        [SerializeField] private float waveRateInSec;
        [SerializeField] private int waveThreatEnhancement;
        [SerializeField] private int minThreatInPercent;
        [SerializeField] private int bossRateMod;
        [SerializeField] private int increasedBossHealthInPercent;

        private int _score;
        private int _wave;
        private int _waveThreat;
        private int _currentThreat;

        private Dictionary<int, GameObject> _enemiesWithThreat;

        public Action<int, int> gameScoreUpdated;
        public Action playerDied;
        public Action<int, int> newWaveHasBegun;
        public Action<int, int> bossSpawned;

        private SoundManager _soundManager;
        private PlayerController _player;
        private float _timeBtwSpawn;


        // Start is called before the first frame update
        void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            _soundManager = GetComponent<SoundManager>();
            InitEnemies();
            _score = 0;
        }
        
        void FixedUpdate()
        {
            if (_player.GetPlayerHealth() <= 0)
            {
                _soundManager.Lose();
                playerDied?.Invoke();
                Stop();
            }
            else
            {
                CheckWave();
            }
        }

        private void InitEnemies()
        {
            _enemiesWithThreat = new Dictionary<int, GameObject>();

            for (var i = 0; i < enemies.Length; i++)
            {
                _enemiesWithThreat.Add(
                    enemies[i].GetComponent<Enemy>().threat,
                    enemies[i]);
            }
        }

        private GameObject GetEnemyByThreat(int threatValue)
        {
            _enemiesWithThreat.TryGetValue(threatValue, out var value);
            return value;
        }

        private void CheckWave()
        {
            if (_timeBtwSpawn <= 0 ||
                _currentThreat <= _waveThreat * minThreatInPercent / 100)
            {
                _timeBtwSpawn = waveRateInSec;
                _wave++;
                _waveThreat += waveThreatEnhancement;
                _currentThreat += _waveThreat;

                if (_wave % bossRateMod == 0)
                {
                    SpawnBoss();
                }
                else
                {
                    SpawnEnemies();
                }
                
                gameScoreUpdated?.Invoke(_score, _currentThreat);
            }
            else
            {
                _timeBtwSpawn -= Time.deltaTime;
            }
        }

        public int GetScore()
        {
            return _score;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
        }

        private void SpawnBoss()
        {
            var rnd = Random.Range(0, bosses.Length);
            var randomPoint = Random.Range(0, spawnPoints.Length);

            var rndBoss = bosses[rnd];
            
            var boss = Instantiate(
                rndBoss,
                new Vector3(
                    spawnPoints[randomPoint].position.x,
                    spawnPoints[randomPoint].position.y + rndBoss.transform.localScale.y / 2,
                    spawnPoints[randomPoint].position.z),
                Quaternion.identity);

            var bossScript = boss.GetComponent<Enemy>();
            bossScript.health = bossScript.health * _wave * increasedBossHealthInPercent / 100;
            
            var rndWeapon = Random.Range(0, weapons.Length);
            bossScript.SetKillAward(weapons[rndWeapon]);

            bossScript.threat = _waveThreat;
            bossScript.enemyKilled += EnemyKilled;

            bossSpawned?.Invoke(_wave, _waveThreat);
        }

        private void SpawnEnemies()
        {
            var totalWaveThreat = 0;

            while (totalWaveThreat < _waveThreat)
            {
                var rnd = Random.Range(0, _enemiesWithThreat.Count);
                var enemyKeyValuePair = _enemiesWithThreat.ElementAt(rnd);
                var randomPoint = Random.Range(0, spawnPoints.Length);

                if (totalWaveThreat + enemyKeyValuePair.Key > _waveThreat)
                {
                    var lastEnemy = GetEnemyByThreat(_waveThreat - totalWaveThreat);
                    if (lastEnemy != null)
                    {
                        lastEnemy = Instantiate(
                            lastEnemy,
                            spawnPoints[randomPoint].position,
                            Quaternion.identity);
                    }
                    else
                    {
                        enemyKeyValuePair = _enemiesWithThreat
                            .Aggregate((l, r) =>
                                l.Key < r.Key ? l : r);

                        lastEnemy = Instantiate(
                            enemyKeyValuePair.Value,
                            spawnPoints[randomPoint].position,
                            Quaternion.identity);
                    }

                    SubscribeToKillEvents(lastEnemy);

                    totalWaveThreat += lastEnemy.GetComponent<Enemy>().threat;
                }
                else
                {
                    var enemy = Instantiate(
                        enemyKeyValuePair.Value,
                        spawnPoints[randomPoint].position,
                        Quaternion.identity);

                    SubscribeToKillEvents(enemy);

                    totalWaveThreat += enemyKeyValuePair.Key;
                }
            }

            newWaveHasBegun?.Invoke(_wave, _waveThreat);
        }

        private void EnemyKilled(Enemy enemy, Transform killPosition, GameObject killAward)
        {
            _score++;
            _currentThreat -= enemy.threat;
            gameScoreUpdated?.Invoke(_score, _currentThreat);

            _soundManager.Death();

            if (killAward == null) return;

            var award = Instantiate(killAward, killPosition.position, Quaternion.identity);
            award.name = killAward.name;
        }

        private void SubscribeToKillEvents(GameObject enemy)
        {
            var enemyScript = enemy.GetComponent<Enemy>();

            if (_score != 0 && _score % lootRate == 0)
            {
                var rndImprovement = Random.Range(0, supplyItems.Length);
                ;
                enemyScript.SetKillAward(supplyItems[rndImprovement]);
            }

            enemyScript.enemyKilled += EnemyKilled;
        }

        private void Stop()
        {
            Time.timeScale = 0;

            _player.enabled = false;
            var enemiesGo = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemiesGo)
            {
                e.GetComponent<Enemy>().enabled = false;
            }

            enabled = false;
        }
    }
}