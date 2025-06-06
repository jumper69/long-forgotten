using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class WalkerGenerator : MonoBehaviour
{
    public enum Grid
    {
        FLOOR,
        WALL,
        EMPTY
    }

    public Grid[,] gridHandler;
    public List<WalkerObject> Walkers;
    public Tilemap tileMap;
    public TileBase Floor;
    public TileBase Wall;
    public Tilemap wallTileMap;
   
    public int MapWidth = 120;
    public int MapHeight = 120;

    public int MaximumWalkers = 30;
    public int TileCount = default;
    public float FillPercentage = 0.5f;
    public float WaitTime = 0.00f;

    public Tilemap plantsTilemap;
    public TileBase PlantTile;
    public float PlantSpawnChance = 0.01f;

    public Tilemap grassTilemap;
    public TileBase GrassTile;
    public float GrassSpawnChance = 0.015f;

    public NavMeshSurface navMeshSurface;

    public GameObject skeletonPrefab;
    public GameObject orcPrefab;
    public int numberOfEnemies = 4;

    public GameObject orcRiderPrefab;

    public GameObject[] treePrefabs;
    public float treeSpawnChance = 0.01f;

    public GameObject playerObject;

    public GameObject[] structurePrefabs;
    public int numberOfStructures = 3;
    public int structureSpacing = 6;

    public GameObject npcObject;

    public LoadingScreenController loadingScreen;

    bool HasFloorNeighbor(Vector3Int pos)
    {
        int x = pos.x;
        int y = pos.y;

        if (x > 0 && gridHandler[x - 1, y] == Grid.FLOOR) return true;
        if (x < MapWidth - 1 && gridHandler[x + 1, y] == Grid.FLOOR) return true;
        if (y > 0 && gridHandler[x, y - 1] == Grid.FLOOR) return true;
        if (y < MapHeight - 1 && gridHandler[x, y + 1] == Grid.FLOOR) return true;

        return false;
    }
    bool HasEnoughFloorNeighbors(Vector3Int pos, int minNeighbors = 1)
    {
        int x = pos.x;
        int y = pos.y;

        int count = 0;

        if (x > 0 && gridHandler[x - 1, y] == Grid.FLOOR) count++;
        if (x < MapWidth - 1 && gridHandler[x + 1, y] == Grid.FLOOR) count++;
        if (y > 0 && gridHandler[x, y - 1] == Grid.FLOOR) count++;
        if (y < MapHeight - 1 && gridHandler[x, y + 1] == Grid.FLOOR) count++;

        return count >= minNeighbors;
    }

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridHandler = new Grid[MapWidth, MapHeight];

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<WalkerObject>();

        Vector3Int TileCenter = new Vector3Int(MapWidth / 2, MapHeight / 2, 0);

        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), 0.5f);
        gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        tileMap.SetTile(TileCenter, Floor);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(StartWithLoadingScreen());
        //StartCoroutine(CreateFloors());
    }

    Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    IEnumerator CreateFloors()
    {
        int iteration = 0;
        int maxIterations = 5000;

        while ((float)TileCount / (float)gridHandler.Length < FillPercentage && iteration < maxIterations)
        {
            iteration++;
            bool hasCreatedFloor = false;

            foreach (WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                int width = UnityEngine.Random.Range(2, 6);
                int height = UnityEngine.Random.Range(2, 6);

                for (int dx = 0; dx < width; dx++)
                {
                    for (int dy = 0; dy < height; dy++)
                    {
                        int newX = curPos.x + dx;
                        int newY = curPos.y + dy;

                        if (newX >= 1 && newX < MapWidth - 1 && newY >= 1 && newY < MapHeight - 1)
                        {
                            Vector3Int newPos = new Vector3Int(newX, newY, 0);

                            if (gridHandler[newX, newY] != Grid.FLOOR &&
                                (TileCount < 100 || HasEnoughFloorNeighbors(newPos, 1)))
                            {
                                gridHandler[newX, newY] = Grid.FLOOR;
                                tileMap.SetTile(newPos, Floor);
                                TileCount++;
                                hasCreatedFloor = true;
                            }
                        }
                    }
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasCreatedFloor)
            {
                yield return new WaitForSeconds(WaitTime);
            }
            else
            {
                yield return null;
            }

            float fillPercent = (float)TileCount / (float)(MapWidth * MapHeight);
            if (loadingScreen != null)
            {
                loadingScreen.SetProgress(Mathf.Lerp(0f, 0.6f, fillPercent));
            }
        }

        Debug.Log("Floor generation finished. Iterations: " + iteration);

        RemoveLonelyTiles();
        RemoveWeaklyConnectedFloors(2);
        FixThinFloorRegions();
        FillLonelyTileNeighbors();
        FillMapWithWalls(30);
        ScatterPlants();
        ScatterGrass();
        ScatterTrees();
        ScatterStructures();

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("NavMeshSurface not assigned!");
        }

        SpawnEnemies();
        SpawnBoss();

        if (playerObject != null)
        {
            Vector3Int spawnTile = FindSafePlayerStart();
            Vector3 worldPos = tileMap.CellToWorld(spawnTile) + new Vector3(0.5f, 0.5f, 0);
            playerObject.transform.position = worldPos;
        }

        if (npcObject != null)
        {
            Vector3Int npcTile = FindSafePlayerStart();

            if (npcTile.x >= 0 && npcTile.y >= 0)
            {
                Vector3 npcWorldPos = tileMap.CellToWorld(npcTile) + new Vector3(0.5f, 0.5f, 0);
                npcObject.transform.position = npcWorldPos;
            }
        }
    }

    void FillLonelyTileNeighbors()
    {
        List<Vector3Int> toAdd = new List<Vector3Int>();

        for (int x = 1; x < MapWidth - 2; x++)
        {
            for (int y = 1; y < MapHeight - 2; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (!HasEnoughFloorNeighbors(pos, 2))
                    {
                        Vector3Int[] potential = new Vector3Int[]
                        {
                        new Vector3Int(x, y + 1, 0),
                        new Vector3Int(x + 1, y, 0),
                        new Vector3Int(x + 1, y + 1, 0)
                        };

                        foreach (var p in potential)
                        {
                            if (gridHandler[p.x, p.y] == Grid.EMPTY)
                            {
                                toAdd.Add(p);
                            }
                        }
                    }
                }
            }
        }

        foreach (var pos in toAdd)
        {
            if (gridHandler[pos.x, pos.y] == Grid.EMPTY)
            {
                gridHandler[pos.x, pos.y] = Grid.FLOOR;
                tileMap.SetTile(pos, Floor);
                TileCount++;
            }
        }
    }
    void FixThinFloorRegions()
    {
        bool[,] visited = new bool[MapWidth, MapHeight];

        for (int x = 1; x < MapWidth - 1; x++)
        {
            for (int y = 1; y < MapHeight - 1; y++)
            {
                if (!visited[x, y] && gridHandler[x, y] == Grid.FLOOR)
                {
                    List<Vector2Int> region = new List<Vector2Int>();
                    Queue<Vector2Int> queue = new Queue<Vector2Int>();
                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x, y] = true;

                    int minX = x, maxX = x, minY = y, maxY = y;

                    while (queue.Count > 0)
                    {
                        Vector2Int current = queue.Dequeue();
                        region.Add(current);

                        minX = Mathf.Min(minX, current.x);
                        maxX = Mathf.Max(maxX, current.x);
                        minY = Mathf.Min(minY, current.y);
                        maxY = Mathf.Max(maxY, current.y);

                        foreach (Vector2Int dir in new Vector2Int[] {
                        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                        {
                            Vector2Int next = current + dir;
                            if (next.x > 0 && next.x < MapWidth && next.y > 0 && next.y < MapHeight)
                            {
                                if (!visited[next.x, next.y] && gridHandler[next.x, next.y] == Grid.FLOOR)
                                {
                                    visited[next.x, next.y] = true;
                                    queue.Enqueue(next);
                                }
                            }
                        }
                    }

                    int width = maxX - minX + 1;
                    int height = maxY - minY + 1;

                    int minWidth = 2;
                    int minHeight = 3;

                    if (width < minWidth || height < minHeight)
                    {
                        foreach (var pos in region)
                        {
                            gridHandler[pos.x, pos.y] = Grid.EMPTY;
                            tileMap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
                            TileCount--;
                        }
                    }
                }
            }
        }
    }
    void RemoveWeaklyConnectedFloors(int minNeighbors = 2)
    {
        for (int x = 1; x < MapWidth - 1; x++)
        {
            for (int y = 1; y < MapHeight - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (!HasEnoughFloorNeighbors(pos, minNeighbors))
                    {
                        gridHandler[x, y] = Grid.EMPTY;
                        tileMap.SetTile(pos, null);
                        TileCount--;
                    }
                }
            }
        }
    }

    void ChanceToRemove()
    {
        for (int i = Walkers.Count - 1; i >= 0; i--)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
        if (Walkers.Count == 0)
        {
            Vector2 center = new Vector2(MapWidth / 2, MapHeight / 2);
            WalkerObject fallback = new WalkerObject(center, GetDirection(), 0.5f);
            Walkers.Add(fallback);
            Debug.LogWarning("All walkers lost! Spawning backup walker.");
        }
    }


    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange)
            {
                WalkerObject curWalker = Walkers[i];
                curWalker.Direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count < MaximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i].Position;

                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.5f);
                Walkers.Add(newWalker);
            }
        }
    }
    void UpdatePosition()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            WalkerObject FoundWalker = Walkers[i];
            FoundWalker.Position += FoundWalker.Direction;
            FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 1, gridHandler.GetLength(0) - 2);
            FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 1, gridHandler.GetLength(1) - 2);
            Walkers[i] = FoundWalker;
        }
    }
    void CreateWalls()
    {
        for (int x = 1; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), Wall);
                        gridHandler[x + 1, y] = Grid.WALL;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), Wall);
                        gridHandler[x - 1, y] = Grid.WALL;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1, 0), Wall);
                        gridHandler[x, y + 1] = Grid.WALL;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1, 0), Wall);
                        gridHandler[x, y - 1] = Grid.WALL;
                    }
                }
            }
        }
    }

    void RemoveLonelyTiles()
    {
        for (int x = 1; x < MapWidth - 1; x++)
        {
            for (int y = 1; y < MapHeight - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR && !HasFloorNeighbor(new Vector3Int(x, y, 0)))
                {
                    gridHandler[x, y] = Grid.EMPTY;
                    tileMap.SetTile(new Vector3Int(x, y, 0), null);
                    TileCount--;
                }
            }
        }
    }

    void FillMapWithWalls(int left = 30, int right = 30, int bottom = 30, int top = 30)
    {
        int startX = -left;
        int endX = MapWidth + right;
        int startY = -bottom;
        int endY = MapHeight + top;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight || gridHandler[x, y] == Grid.EMPTY)
                {
                    wallTileMap.SetTile(pos, Wall);
                }
            }
        }
    }

    void ScatterPlants()
    {
        for (int x = 2; x < gridHandler.GetLength(0) - 2; x++)
        {
            for (int y = 2; y < gridHandler.GetLength(1) - 2; y++)
            {
                if (gridHandler[x, y] != Grid.FLOOR)
                    continue;

                bool isDeepInsideFloor = true;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (gridHandler[x + dx, y + dy] != Grid.FLOOR)
                        {
                            isDeepInsideFloor = false;
                            break;
                        }
                    }
                    if (!isDeepInsideFloor) break;
                }

                if (isDeepInsideFloor && UnityEngine.Random.value < PlantSpawnChance)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    plantsTilemap.SetTile(pos, PlantTile);
                }
            }
        }
    }
    void ScatterGrass()
    {
        for (int x = 2; x < gridHandler.GetLength(0) - 2; x++)
        {
            for (int y = 2; y < gridHandler.GetLength(1) - 2; y++)
            {
                if (gridHandler[x, y] != Grid.FLOOR)
                    continue;

                bool isDeepInsideFloor = true;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (gridHandler[x + dx, y + dy] != Grid.FLOOR)
                        {
                            isDeepInsideFloor = false;
                            break;
                        }
                    }
                    if (!isDeepInsideFloor) break;
                }

                if (isDeepInsideFloor && UnityEngine.Random.value < GrassSpawnChance)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    grassTilemap.SetTile(pos, GrassTile);
                }
            }
        }
    }
    void SpawnEnemies()
    {
        int totalWidth = gridHandler.GetLength(0);
        int totalHeight = gridHandler.GetLength(1);

        List<Vector3Int> floorPositions = new List<Vector3Int>();

        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    floorPositions.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int i = 0; i < numberOfEnemies && floorPositions.Count > 0; i++)
        {
            int randIndex = Random.Range(0, floorPositions.Count);
            Vector3Int spawnPos = floorPositions[randIndex];
            floorPositions.RemoveAt(randIndex);

            GameObject enemyToSpawn = (i % 2 == 0) ? skeletonPrefab : orcPrefab;

            Vector3 worldPos = tileMap.CellToWorld(spawnPos) + new Vector3(0.5f, 0.5f, 0);

            Instantiate(enemyToSpawn, worldPos, Quaternion.identity);
        }
    }

    void SpawnBoss()
    {
        List<Vector3Int> floorPositions = new List<Vector3Int>();

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    floorPositions.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        if (floorPositions.Count > 0)
        {
            int randIndex = Random.Range(0, floorPositions.Count);
            Vector3Int spawnTile = floorPositions[randIndex];
            Vector3 worldPos = tileMap.CellToWorld(spawnTile) + new Vector3(0.5f, 0.5f, 0);
            Instantiate(orcRiderPrefab, worldPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No valid floor.");
        }
    }

    void ScatterTrees()
    {
        int minSpacing = 2;
        HashSet<Vector3Int> occupied = new HashSet<Vector3Int>();

        for (int x = minSpacing; x < gridHandler.GetLength(0) - minSpacing; x++)
        {
            for (int y = minSpacing; y < gridHandler.GetLength(1) - minSpacing; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (gridHandler[x, y] != Grid.FLOOR)
                    continue;

                bool tooCloseToTree = false;
                for (int dx = -minSpacing; dx <= minSpacing && !tooCloseToTree; dx++)
                {
                    for (int dy = -minSpacing; dy <= minSpacing; dy++)
                    {
                        Vector3Int neighbor = new Vector3Int(x + dx, y + dy, 0);
                        if (occupied.Contains(neighbor))
                        {
                            tooCloseToTree = true;
                            break;
                        }
                    }
                }
                if (tooCloseToTree) continue;

                bool safeGround = true;
                for (int dx = -minSpacing; dx <= minSpacing && safeGround; dx++)
                {
                    for (int dy = -minSpacing; dy <= minSpacing; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (gridHandler[nx, ny] != Grid.FLOOR)
                        {
                            safeGround = false;
                            break;
                        }
                    }
                }
                if (!safeGround) continue;

                if (Random.value < treeSpawnChance)
                {
                    Vector3 worldPos = tileMap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0);
                    int index = Random.Range(0, treePrefabs.Length);
                    GameObject tree = Instantiate(treePrefabs[index], worldPos, Quaternion.identity);
                    tree.isStatic = true;

                    occupied.Add(cell);
                }
            }
        }
    }

    Vector3Int FindSafePlayerStart()
    {
        int edgeOffset = 3;
        int maxScanDepth = 10;

        for (int depth = edgeOffset; depth < edgeOffset + maxScanDepth; depth++)
        {
            for (int x = depth; x < MapWidth - depth; x++)
            {
                for (int y = depth; y < MapHeight - depth; y++)
                {
                    if (gridHandler[x, y] != Grid.FLOOR) continue;

                    bool isSafe = true;
                    for (int dx = -1; dx <= 1 && isSafe; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx < 0 || ny < 0 || nx >= MapWidth || ny >= MapHeight || gridHandler[nx, ny] != Grid.FLOOR)
                            {
                                isSafe = false;
                                break;
                            }
                        }
                    }

                    if (isSafe)
                        return new Vector3Int(x, y, 0);
                }
            }
        }

        Debug.LogWarning("Couldn't find safe edge spawn, using center.");
        return new Vector3Int(MapWidth / 2, MapHeight / 2, 0);
    }


    void ScatterStructures()
    {
        int structureWidth = 7;
        int structureHeight = 10;
        int spacing = structureSpacing;
        int attempts = 100;
        int placed = 0;

        while (placed < numberOfStructures && attempts-- > 0)
        {
            int x = Random.Range(spacing, MapWidth - structureWidth - spacing);
            int y = Random.Range(spacing, MapHeight - structureHeight - spacing);

            bool canPlace = true;

            for (int dx = 0; dx < structureWidth && canPlace; dx++)
            {
                for (int dy = 0; dy < structureHeight; dy++)
                {
                    int checkX = x + dx;
                    int checkY = y + dy;

                    if (gridHandler[checkX, checkY] != Grid.FLOOR)
                    {
                        canPlace = false;
                        break;
                    }

                    Vector3 worldPos = tileMap.CellToWorld(new Vector3Int(checkX, checkY, 0)) + new Vector3(0.5f, 0.5f, 0);
                    Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.4f, LayerMask.GetMask("Obstacles"));
                    if (hit != null)
                    {
                        canPlace = false;
                        break;
                    }
                }
            }

            if (!canPlace) continue;

            GameObject prefab = structurePrefabs[Random.Range(0, structurePrefabs.Length)];
            Vector3Int centerTile = new Vector3Int(x + structureWidth / 2, y + structureHeight / 2, 0);
            Vector3 worldPosition = tileMap.CellToWorld(centerTile) + new Vector3(0.5f, 0.5f, 0);

            Instantiate(prefab, worldPosition, Quaternion.identity);
            placed++;
        }
    }

    void PlacePlayerNearNPC()
    {
        GameObject npc = GameObject.FindWithTag("NPC");
        if (npc == null)
        {
            Debug.LogWarning("NPC not found, falling back to edge.");
            return;
        }

        Vector3 npcWorldPos = npc.transform.position;
        Vector3Int npcTile = tileMap.WorldToCell(npcWorldPos);

        int searchRadius = 5;

        for (int r = 1; r <= searchRadius; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    int x = npcTile.x + dx;
                    int y = npcTile.y + dy;

                    if (x >= 0 && x < MapWidth && y >= 0 && y < MapHeight)
                    {
                        if (gridHandler[x, y] == Grid.FLOOR)
                        {
                            Vector3 worldPos = tileMap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0);
                            playerObject.transform.position = worldPos;
                            Debug.Log("Player placed near NPC.");
                            return;
                        }
                    }
                }
            }
        }

        Debug.LogWarning("No terrain near npc.");
    }

    IEnumerator StartWithLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.Show();
            loadingScreen.SetProgress(0.05f);
        }

        yield return CreateFloors();

        if (loadingScreen != null) loadingScreen.SetProgress(0.5f);

        ScatterPlants();
        if (loadingScreen != null) loadingScreen.SetProgress(0.7f);
        yield return null;
        ScatterGrass();
        if (loadingScreen != null) loadingScreen.SetProgress(0.75f);
        yield return null;
        ScatterTrees();
        if (loadingScreen != null) loadingScreen.SetProgress(0.8f);
        yield return null;
        ScatterStructures();
        if (loadingScreen != null) loadingScreen.SetProgress(0.85f);
        yield return null;

        if (loadingScreen != null) loadingScreen.SetProgress(0.9f);

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        if (loadingScreen != null) loadingScreen.SetProgress(0.9f);

        SpawnEnemies();
        SpawnBoss();
        if (loadingScreen != null) loadingScreen.SetProgress(0.95f);
        yield return null;

        PlacePlayerNearNPC();
        if (loadingScreen != null)
        {
            loadingScreen.SetProgress(1f);
            yield return new WaitForSeconds(0.5f);
            loadingScreen.Hide();
        }
    }
}