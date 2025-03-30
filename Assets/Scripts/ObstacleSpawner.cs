using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Pathfinding;
public class ObstacleSpawner : MonoBehaviour
{
    public GameObject destructiblePrefab;
    public GameObject indestructiblePrefab;
    public Tilemap groundTilemap;
    public Tilemap invisibleWallTilemap;
    public LayerMask obstacleLayer;


    public int gridSize = 23;
    public int destructibleCount = 15;
    public int indestructibleCount = 10;

    private HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();

    void Start()
    {
        SpawnObstacles(destructiblePrefab, destructibleCount);
        SpawnObstacles(indestructiblePrefab, indestructibleCount);
    }

    void SpawnObstacles(GameObject prefab, int count)
    {
        List<Vector3Int> validPositions = new List<Vector3Int>();
        BoundsInt bounds = groundTilemap.cellBounds;
        
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                
                // Exclude outermost layer: border walls
                // Skip if no ground tile or if blocked by invisible wall
                if (!groundTilemap.HasTile(pos)) continue;
                if (invisibleWallTilemap.HasTile(pos)) continue;
                
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
                // Use OverlapBox (or OverlapCircle) to detect obstacles.
                // Adjust the size (here using 1x1) to fit your obstacle size.
                Collider2D obstacleCheck = Physics2D.OverlapBox(worldPos, new Vector2(1f, 1f), 0f, obstacleLayer);
                if (obstacleCheck != null)
                    continue;

                
                if (groundTilemap.HasTile(pos))
                    validPositions.Add(pos);
            }
        }

        // Shuffle
        for (int i = 0; i < validPositions.Count; i++)
        {
            int rand = Random.Range(i, validPositions.Count);
            (validPositions[i], validPositions[rand]) = (validPositions[rand], validPositions[i]);
        }

        // Place prefabs
        int placed = 0;
        foreach (Vector3Int pos in validPositions)
        {
            if (placed >= count) break;

            Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);
            Instantiate(prefab, worldPos, Quaternion.identity);
            placed++;
        }
        AstarPath.active.Scan();
    }




}