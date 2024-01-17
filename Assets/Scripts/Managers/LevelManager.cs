using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Api.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    /// !!! WARNING !!!
    /// <summary>
    /// Each enemy prefab must have a unique threat value
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        public GameObject[] enemies;
        public GameObject[] bosses;
        public Transform[] spawnPoints;
        public int minSpawnDistanceToPlayer;

        [Header("LOOT:")] [SerializeField] private GameObject[] supplyItems;
        [SerializeField] private GameObject[] weapons;
        [SerializeField] private GameObject nftItem;
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

        /// <summary>
        /// Store enemies by threat
        /// </summary>
        /// <typeparam name="int">Enemy threat</typeparam>
        /// <typeparam name="GameObject">Enemy prefab</typeparam>
        private Dictionary<int, GameObject> _enemiesWithThreat;

        public Action<int, int> gameScoreUpdated;
        public Action playerDied;
        public Action<int, int> newWaveHasBegun;
        public Action<int, int> bossSpawned;

        private SoundManager _soundManager;
        private PlayerController _player;
        private float _timeBtwSpawn;
        private float _distanceBtwItemDrop = 2f;

        private GameSession _gameSession;


        // Start is called before the first frame update
        void Start()
        {
            UserDataManager.Instance.GameStarted += GameStarted;
            _player = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<PlayerController>();
            _soundManager = GetComponent<SoundManager>();
            InitEnemies();
            _score = 0;
        }

        private void GameStarted(GameSession session)
        {
            if (!session.IsNew) return;
            
            _gameSession = session;
        }

        private void FixedUpdate()
        {
            if (_player.GetPlayerHealth() <= 0)
            {
                EndGame();
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
            var spawnPoint = GetRandomSpawnPoint();

            var rndBoss = bosses[rnd];
            
            var boss = Instantiate(
                rndBoss,
                new Vector3(
                    spawnPoint.x,
                    spawnPoint.y + rndBoss.transform.localScale.y / 2,
                    spawnPoint.z),
                Quaternion.identity);

            var bossScript = boss.GetComponent<Enemy>();
            bossScript.health = bossScript.health * _wave * increasedBossHealthInPercent / 100;

            var rndWeapon = Random.Range(0, weapons.Length);
            bossScript.AddKillAward(weapons[rndWeapon]);

            if (_gameSession != null)
            {
                var bossIndex = _wave / bossRateMod;
                bossScript.AppointBoss(bossIndex);
                var drop = _gameSession
                    .GameDrop
                    .FirstOrDefault(gd => gd.Boss == bossIndex);
                if (drop != null)
                    bossScript.AddKillAward(nftItem);
            }

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
                var spawnPoint = GetRandomSpawnPoint();

                if (totalWaveThreat + enemyKeyValuePair.Key > _waveThreat)
                {
                    var lastEnemy = GetEnemyByThreat(_waveThreat - totalWaveThreat);
                    if (lastEnemy != null)
                    {
                        lastEnemy = Instantiate(
                            lastEnemy,
                            spawnPoint,
                            Quaternion.identity);
                    }
                    else
                    {
                        enemyKeyValuePair = _enemiesWithThreat
                            .Aggregate((l, r) =>
                                l.Key < r.Key ? l : r);

                        lastEnemy = Instantiate(
                            enemyKeyValuePair.Value,
                            spawnPoint,
                            Quaternion.identity);
                    }

                    SubscribeToKillEvents(lastEnemy);

                    totalWaveThreat += lastEnemy.GetComponent<Enemy>().threat;
                }
                else
                {
                    var enemy = Instantiate(
                        enemyKeyValuePair.Value,
                        spawnPoint,
                        Quaternion.identity);

                    SubscribeToKillEvents(enemy);

                    totalWaveThreat += enemyKeyValuePair.Key;
                }
            }

            newWaveHasBegun?.Invoke(_wave, _waveThreat);
        }

        private Vector3 GetRandomSpawnPoint()
        {
            var randomPoint = Random.Range(0, spawnPoints.Length);
            while (Vector3.Distance(
                       _player.transform.position,
                       spawnPoints[randomPoint].position) 
                   < minSpawnDistanceToPlayer)
            {
                randomPoint = Random.Range(0, spawnPoints.Length);
            }

            return spawnPoints[randomPoint].position;
        }

        private void EnemyKilled(Enemy enemy, Transform killPosition, List<GameObject> killAwards)
        {
            _score++;
            _currentThreat -= enemy.threat;
            gameScoreUpdated?.Invoke(_score, _currentThreat);

            _soundManager.Death();

            for (var i = 0; i < killAwards.Count; i++)
            {
                var position = killPosition.position;
                
                position = new Vector3(
                    position.x + _distanceBtwItemDrop * i,
                    position.y, 
                    position.z + _distanceBtwItemDrop * i);

                var award = Instantiate(killAwards[i], position, Quaternion.identity);
                award.name = killAwards[i].name;
            }
            
            if (!enemy.IsTheBoss()) return;
            
            UserDataManager.Instance.KillBoss(
                _gameSession.GameId,
                enemy.GetBossIndex());
        }

        private void SubscribeToKillEvents(GameObject enemy)
        {
            var enemyScript = enemy.GetComponent<Enemy>();

            if (_score != 0 && _score % lootRate == 0)
            {
                var rndImprovement = Random.Range(0, supplyItems.Length);
                enemyScript.AddKillAward(supplyItems[rndImprovement]);
            }

            enemyScript.enemyKilled += EnemyKilled;
        }

        private void StopSceneScripts()
        {
            Time.timeScale = 0;

            _player.enabled = false;
            
            var enemiesGo = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemiesGo)
                e.GetComponent<Enemy>().enabled = false;

            enabled = false;
        }

        private void EndGame()
        {
            UserDataManager.Instance.EndGame(_gameSession.GameId);
            _soundManager.Lose();
            playerDied?.Invoke();
            StopSceneScripts();
        }

        public void LoadScene(string scene){
            if (scene != "")
            {
                StartCoroutine(LoadAsynchronously(scene));
            }
        }
        
        private IEnumerator LoadAsynchronously(string scene)
        {
            if (scene == "") yield break;
            
            Time.timeScale = 1;
            var asyncLoad = SceneManager.LoadSceneAsync(scene);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}