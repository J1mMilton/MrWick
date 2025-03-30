using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : MonoBehaviour
{
    // Array to hold different types of items (e.g. health packs, power-ups)
    public GameObject[] itemPrefabs;
    public Tilemap groundTilemap;
    // Optional: if you have a tilemap for obstacles you want to avoid, assign it here.
    public Tilemap obstacleTilemap;
    
    // Number of items you want to spawn in the level.
    public int numberOfItems = 5;

    [Tooltip("Avoid placing items too close to the player's spawn position")]
    public Vector3 playerSpawnPosition;
    public float safeRadius = 3f;

    private List<Vector3Int> validPositions = new List<Vector3Int>();

    void Start()
    {
        GenerateValidPositions();
        SpawnItems();
    }

    // Gather valid positions from the ground tilemap.
    void GenerateValidPositions()
    {
        validPositions.Clear();
        BoundsInt bounds = groundTilemap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                
                // Optionally, exclude the outer border (if needed)
                if (x == bounds.xMin || x == bounds.xMax - 1 || 
                    y == bounds.yMin || y == bounds.yMax - 1)
                    continue;

                // Only use cells that have a ground tile.
                if (!groundTilemap.HasTile(pos)) continue;
                
                // Optionally avoid positions that are occupied by obstacles.
                if(obstacleTilemap != null && obstacleTilemap.HasTile(pos)) continue;

                // Avoid positions too close to the player spawn.
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                if (Vector3.Distance(worldPos, playerSpawnPosition) < safeRadius)
                    continue;

                validPositions.Add(pos);
            }
        }

        // Shuffle the list to randomize item placements.
        for (int i = 0; i < validPositions.Count; i++)
        {
            int rand = Random.Range(i, validPositions.Count);
            Vector3Int temp = validPositions[i];
            validPositions[i] = validPositions[rand];
            validPositions[rand] = temp;
        }
    }

    // Spawn items at some of the valid positions.
    void SpawnItems()
    {
        int placed = 0;
        foreach (var pos in validPositions)
        {
            if (placed >= numberOfItems) break;
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
            
            // Randomly pick one item type from the array.
            GameObject chosenItem = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Instantiate(chosenItem, worldPos, Quaternion.identity);
            placed++;
        }
    }
}
