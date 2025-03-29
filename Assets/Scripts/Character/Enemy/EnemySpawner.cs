using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Tilemap groundTilemap;
    public Tilemap invisibleWallTilemap;
    public int numberOfEnemies = 10;
    
    private int enemiesRemaining;

    public GameObject winCanvas; // drag this in the Inspector

    [Tooltip("Avoid placing enemies within this distance of the player spawn")]
    public Vector3 playerSpawnPosition;
    public float safeRadius = 3f;
    
    public Transform playerTransform;

    void Start()
    {
        SpawnEnemies();
        enemiesRemaining = numberOfEnemies;

        if (winCanvas != null)
            winCanvas.SetActive(false); // Hide it at start

    }

    void SpawnEnemies()
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Exclude outer border
                if (x == bounds.xMin || x == bounds.xMax - 1 ||
                    y == bounds.yMin || y == bounds.yMax - 1)
                    continue;

                // Skip if no ground tile or blocked by invisible wall
                if (!groundTilemap.HasTile(pos)) continue;
                if (invisibleWallTilemap.HasTile(pos)) continue;

                // Skip if too close to the player spawn
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                if (Vector3.Distance(worldPos, playerSpawnPosition) < safeRadius) continue;

                validPositions.Add(pos);
            }
        }

        // Shuffle
        for (int i = 0; i < validPositions.Count; i++)
        {
            int rand = Random.Range(i, validPositions.Count);
            (validPositions[i], validPositions[rand]) = (validPositions[rand], validPositions[i]);
        }

        // Spawn enemies
        int placed = 0;
        foreach (var pos in validPositions)
        {
            if (placed >= numberOfEnemies) break;

            Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
            
            // Set the player reference manually
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.SetTarget(playerTransform); // <- you’ll add this method below
            placed++;
        }
    }
    
    public void EnemyDied()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            WinLevel();
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
