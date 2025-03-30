using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;               // For Level 1
    public GameObject[] level2EnemyPrefabs;        // For Level 2
    public Tilemap groundTilemap;
    public Tilemap invisibleWallTilemap;
    
    // For Level 1 (instant spawn)
    public int numberOfEnemies = 10; 

    // For Level 2 (progressive mode)
    public int initialEnemies = 10;      // Number spawned initially in Level 2
    public int totalEnemiesForLevel2 = 20; // Total enemies that will be spawned in Level 2

    // Tracking variables
    private int enemiesSpawned = 0;  // Total number of enemies that have been spawned
    private int killsCount = 0;      // Total number of enemies killed
    private int currentAlive = 0;    // Current number of enemies alive
    private bool progressiveMode = false; // false for Level 1, true for Level 2

    public GameObject winCanvas; // Drag your WinCanvas here in the Inspector

    [Tooltip("Avoid placing enemies within this distance of the player spawn")]
    public Vector3 playerSpawnPosition;
    public float safeRadius = 3f;
    
    public Transform playerTransform;

    private List<Vector3Int> validPositions = new List<Vector3Int>();

    void Start()
    {
        // Hide WinCanvas at start.
        if (winCanvas != null)
            winCanvas.SetActive(false);

        // Build valid spawn positions.
        GenerateValidPositions();

        // Determine mode based on scene name.
        if (SceneManager.GetActiveScene().name == "Scene_World_02")
        {
            progressiveMode = true;
            // For Level 2, spawn only the initialEnemies.
            killsCount = 0;
            currentAlive = 0;
            enemiesSpawned = 0;
            for (int i = 0; i < initialEnemies; i++)
            {
                SpawnOneEnemy();
                currentAlive++;
            }
        }
        else
        {
            progressiveMode = false;
            // For Level 1, spawn all enemies immediately.
            for (int i = 0; i < numberOfEnemies; i++)
            {
                SpawnOneEnemy();
                currentAlive++;
            }
        }
    }

    // Generate valid positions from the tilemap.
    void GenerateValidPositions()
    {
        validPositions.Clear();
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Exclude the outer border.
                if (x == bounds.xMin || x == bounds.xMax - 1 ||
                    y == bounds.yMin || y == bounds.yMax - 1)
                    continue;

                if (!groundTilemap.HasTile(pos)) continue;
                if (invisibleWallTilemap.HasTile(pos)) continue;

                Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                if (Vector3.Distance(worldPos, playerSpawnPosition) < safeRadius)
                    continue;

                validPositions.Add(pos);
            }
        }

        // Shuffle the valid positions for randomness.
        for (int i = 0; i < validPositions.Count; i++)
        {
            int rand = Random.Range(i, validPositions.Count);
            Vector3Int temp = validPositions[i];
            validPositions[i] = validPositions[rand];
            validPositions[rand] = temp;
        }
    }

    // Spawns one enemy at a random valid position.
    void SpawnOneEnemy()
    {
        if (validPositions.Count == 0)
            return;

        Vector3Int pos = validPositions[Random.Range(0, validPositions.Count)];
        Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);

        GameObject chosenPrefab;
        if (progressiveMode)  // Level 2
        {
            chosenPrefab = level2EnemyPrefabs[Random.Range(0, level2EnemyPrefabs.Length)];
        }
        else // Level 1
        {
            chosenPrefab = enemyPrefab;
        }
        
        GameObject enemy = Instantiate(chosenPrefab, worldPos, Quaternion.identity);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.SetTarget(playerTransform);
        enemyScript.SetSpawner(this); // Pass a reference to this spawner.

        enemiesSpawned++; // Count this spawn.
    }

    // This method should be called by the enemy (via its controller) when it dies.
    public void EnemyDied()
    {
        killsCount++;
        currentAlive--;

        if (progressiveMode)
        {
            // In Level 2, if we haven't reached the total spawn count, spawn a replacement.
            if (enemiesSpawned < totalEnemiesForLevel2)
            {
                SpawnOneEnemy();
                currentAlive++;  // Increase alive count because we spawned a new enemy.
            }

            // Win condition: total kills equal totalEnemiesForLevel2.
            if (killsCount >= totalEnemiesForLevel2)
            {
                WinLevel();
            }
        }
        else
        {
            // In Level 1, win when all spawned enemies have been killed.
            if (killsCount >= numberOfEnemies)
            {
                WinLevel();
            }
        }
    }

    void WinLevel()
    {
        StartCoroutine(WinCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        if (winCanvas != null)
            winCanvas.SetActive(true);
    }
}
