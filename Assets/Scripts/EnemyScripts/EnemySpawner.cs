using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    private class EnemySettings
    {
        [TagSelector] public string EnemyTag;

        [Header("Chance to spawn between 0 and 1")]
        public float ChanceToSpawn;
    }
    private static ObjectPool _enemyObjectPool;
    [SerializeField] private List<EnemySettings> _enemySettings;

    [SerializeField] private Collider2D _spawnZone;

    [SerializeField] private float _spawnDelay;
    
    [SerializeField] private int _spawnCount;

    public GameObject CowPrefab;
    [SerializeField] private float _cowSpawnDelay;

    [HideInInspector] public bool IsSpawning = true;
    [HideInInspector] public bool HasCowSpawned = false;

    [SerializeField] private EnemyWave[] _waves = new EnemyWave[] {};
    [SerializeField] private Gamemode _spawnMode = Gamemode.WaveSpawn;

    private static int _enemyCount = 0;
    public static int EnemyCount { get { return _enemyCount; } set { _enemyCount = value; } }
    
    public enum Gamemode
    {
        EndlessSpawn,
        WaveSpawn
    }
    void Start()
    {
        switch(PlayerPrefs.GetString("Mode", "Story"))
        {
            case "Story":
                _spawnMode = Gamemode.WaveSpawn;
                break;
            case "EndlessEasy":
                _spawnMode = Gamemode.EndlessSpawn;
                break;
            case "EndlessNormal":
                _spawnMode = Gamemode.EndlessSpawn;
                break;
            case "EndlessHard":
                _spawnMode = Gamemode.EndlessSpawn;
                break;
        }

        DeactivateAllEnemies();
        _enemyObjectPool = PoolManager.EnemyPool;
        switch (_spawnMode)
        {
            case Gamemode.WaveSpawn:
                StartCoroutine(SpawnWaves());
                break;
            case Gamemode.EndlessSpawn:
                StartCoroutine(SpawnInsideZone());
                StartCoroutine(WaitAndSpawnCow());
                break;
            default:
                Debug.Log("Default invoke enemySpawner");
                break;
        }
    }
    IEnumerator SpawnWaves()
    {
        foreach(var wave in _waves)
        {
            foreach (var wavePart in wave.WaveParts)
            {
                for (int i = 0; i < wavePart.EnemyCount; i++)
                {
                    SpawnEnemy(wavePart.EnemyTag);
                    ++_enemyCount;
                    yield return new WaitForSeconds(wavePart.DelayBetweenSpawn);
                }
                yield return new WaitUntil(() => _enemyCount == 0);
            }
        }
    }
    private void DeactivateAllEnemies()
    {
        if (_enemyObjectPool == null) 
            return;
        
        foreach(GameObject enemy in _enemyObjectPool.Pool)
        {
            enemy.SetActive(false);
        }
    }

    //private bool IsAllEnemiesDead()
    //{
    //    foreach (var pool in enemyObjectPool.pool)
    //    {
    //        if(pool.activeSelf) return false;
    //    }
    //    return true;
    //}
    IEnumerator SpawnInsideZone()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnDelay);
            if (IsSpawning)
            {
                for (int i = 0; i < _spawnCount; i++)
                {
                    SpawnEnemy();
                }
            }
        }

    }

    IEnumerator WaitAndSpawnCow()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cowSpawnDelay);
            if (!HasCowSpawned)
            {
                IsSpawning = false;
                SpawnCow();
                HasCowSpawned = true;
            }
        }
    }

    Vector2 GetRandomPointInsideTheArea(Collider2D collider)
    {
        float randomX = UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x);
        float randomY = UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y);
        var point = new Vector2(randomX, randomY);
        return point;
    }
    void SpawnEnemy()
    {
        var obj = ChooseObject(_enemySettings); 
        obj.transform.position = GetRandomPointInsideTheArea(_spawnZone);
    }
    void SpawnEnemy(string tag)
    {
        var obj = _enemyObjectPool.GetPooledObjectByTag(tag); 
        obj.transform.position = GetRandomPointInsideTheArea(_spawnZone);
    }

    private void SpawnCow()
    {
        Vector2 cowSpawnPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, 100));
        GameObject.Instantiate(CowPrefab, cowSpawnPosition, Quaternion.identity);
    }

    static GameObject ChooseObject(List<EnemySettings> enemies)
    {
        float totalProbability = 0;
        foreach (var probability in enemies)
        {
            totalProbability += probability.ChanceToSpawn;
        }

        float cumulativeProbability = 0;
        float randomNum = UnityEngine.Random.Range(0f, 1f);

        foreach (var obj in enemies)
        {
            cumulativeProbability += obj.ChanceToSpawn / totalProbability;
            if (randomNum < cumulativeProbability)
            {
                GameObject rez = _enemyObjectPool.GetPooledObjectByTag(obj.EnemyTag);
                return rez;
            }
        }

        return null; // In case of error or no object selected
    }
}
