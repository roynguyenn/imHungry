using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    private double _time;
    private double _period = 10;
    private int _maxEnemiesToSpawn = 3;
    private double _spawnChance = 1;

    public Animator animator;
    public GameObject apple;
    public GameObject mainShooter;
    public GameObject waveShooter;
    public playerMechanics mechanics;
    public CircleCollider2D collider;
    public SpawnerScript mainShooterScript;
    private HungerBar _hungerBar;
    private Dictionary<GameObject, double> _enemyPrefabs = new Dictionary<GameObject, double>();

    void Start()
    {
        _hungerBar = FindObjectOfType<HungerBar>();
        LoadEnemyPrefabs();
    }

    private void LoadEnemyPrefabs()
    {
        _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/BasicEnemy"), 1);
        _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/BasicShooterEnemy"), 1);
        
    }

    private GameObject PickRandomEnemyPrefab()
    {
        double totalWeight = 0;
        foreach (var weight in _enemyPrefabs.Values)
            totalWeight += weight;

        double randomValue = UnityEngine.Random.value * totalWeight;
        foreach (var entry in _enemyPrefabs)
        {
            if (randomValue < entry.Value)
                return entry.Key;
            randomValue -= entry.Value;
        }
        return null;
    }

    private GameObject CreateEnemyInstance(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition(enemyPrefab);
        if (inCircle(collider, spawnPosition)){
            Debug.Log("true");
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            return enemyInstance;
        }
        
        return null;
    }
    private bool inCircle(CircleCollider2D collider, Vector3 position){
        Vector3 center = (Vector2) collider.transform.position +(collider.offset * collider.transform.localScale.x);
        float radius = collider.radius * collider.transform.localScale.x;

        Vector3 distance = position - center;
        if (distance.magnitude <= radius){
            return true;
        }
        return false;

    }

    private Vector3 GetRandomSpawnPosition(GameObject enemyPrefab)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return Vector3.zero;

        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;

        SpriteRenderer spriteRenderer = enemyPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            return Vector3.zero;

        Vector2 spriteSize = spriteRenderer.size;
        float xOffset = spriteSize.x / 2;
        float yOffset = spriteSize.y / 2;
        float padding = 1.0f;

        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        float spawnDistance = Mathf.Sqrt(Mathf.Pow(camWidth / 2, 2) + Mathf.Pow(camHeight / 2, 2)) + Mathf.Max(xOffset, yOffset) + padding;
        float randomAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

        return playerPosition + new Vector3(Mathf.Cos(randomAngle) * spawnDistance, Mathf.Sin(randomAngle) * spawnDistance, 0);
    }

    void Update()
    {
        if (_time > _period)
        {
            _time = 0;
            if (UnityEngine.Random.value < _spawnChance)
            {
                int enemiesToSpawn = UnityEngine.Random.Range(1, _maxEnemiesToSpawn + 1);
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    GameObject randomEnemyPrefab = PickRandomEnemyPrefab();
                    if (randomEnemyPrefab != null)
                    {
                        GameObject newEnemy = CreateEnemyInstance(randomEnemyPrefab);
                        // Debug.Log($"Spawned enemy: {newEnemy.GetComponent<Enemy>().Name}");
                    }
                }
            }
        }
        _time += Time.deltaTime;
    }

    public void IncreaseDifficulty(int newLevel)
    {
        switch (newLevel)
        {
            case 2:
                _period--;
                
                _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/SpreadShooterClose"), 1);
                _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/HomingEnemy"), 1);

                _enemyPrefabs.Remove(Resources.Load<GameObject>("Enemies/BasicShooterEnemy"));
                mechanics.damageTaken += 5;
                

                break;
            case 3:
                _period--;
                _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/EvadingEnemy"), 1);
                _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/SprintingEnemy"), 1);
                mainShooter.SetActive(true);
                apple.SetActive(true);

                _enemyPrefabs.Remove(Resources.Load<GameObject>("Enemies/BasicEnemy"));
                animator.SetBool("stage2", true);

                mechanics.damageTaken += 5;
                break;
            case 4:
                _period --;
                _enemyPrefabs.Remove(Resources.Load<GameObject>("Enemies/HomingEnemy"));
                _enemyPrefabs.Remove(Resources.Load<GameObject>("Enemies/EvadingEnemy"));
                _enemyPrefabs.Remove(Resources.Load<GameObject>("Enemies/SpreadShooterClose"));

                mechanics.damageTaken += 5;
                upgradeShooter();
                break;
            case 5:
                _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/EvadingEnemy"), 1);
                _enemyPrefabs.Add(Resources.Load<GameObject>("Enemies/BasicEnemy"), 3);

                mainShooter.SetActive(false);
                waveShooter.SetActive(true);

                animator.SetBool("stage3", true);
                animator.SetBool("stage2", false);
                
                mechanics.damageTaken += 5;
                break;
        }
    }

    public void upgradeShooter(){
        mainShooterScript.firingRate = 0.4f;
        mainShooterScript.bulletSpeed = 3f;
        mainShooterScript.spreadCount = 10;

    }
}
