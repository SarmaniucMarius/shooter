using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tile_State
{
    EMPTY,
    NOT_EMPTY,
    VISITED
}

public struct Tile
{
    public Transform transform;
    public Vector2Int tileMapPos;
    public Tile_State state;
}

public class MapGenerator : MonoBehaviour
{
    [Serializable]
    public class Map
    {
        public Vector2Int maxMapSize; // always rebake the navmesh if this is changed!
        public Vector2Int mapSize;
        [Range(0, 1)]
        public float outline;
        public int seed;
        [Range(0, 1)]
        public float obstacles;
        public int obstacleMinHeight;
        public int obstacleMaxHeight;
        public Color topColor;
        public Color bottomColor;
    }

    public Map[] maps;
    Map map;
    public int currentMap;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;

    Tile[,] tiles;

    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        map = maps[currentMap];
        GetComponent<BoxCollider>().size = new Vector3(map.mapSize[0], 0.05f, map.mapSize[1]);
        // stupid stuff to make the tilemap editable in the editor
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // tilemap creation
        tiles = new Tile[map.mapSize[1], map.mapSize[0]];
        int zCount = map.mapSize[1];
        int xCount = map.mapSize[0];
        for (int z = 0, i = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++, i++)
            {
                // quads are 1 by 1 units long.
                Vector3 tilePosition = new Vector3(-xCount / 2.0f + 0.5f + x, 0.0f, -zCount / 2.0f + 0.5f + z);
                Transform tile = (Transform)Instantiate(tilePrefab, tilePosition, tilePrefab.rotation);
                tile.localScale = Vector3.one * (1 - map.outline);
                tile.parent = mapHolder;

                tiles[z, x].transform = tile;
                tiles[z, x].tileMapPos = new Vector2Int(x, z);
                tiles[z, x].state = Tile_State.EMPTY;
            }
        }

        // obstacle generation and placement
        System.Random prng = new System.Random(map.seed);
        Tile[] shuffledTiles = FlattenArray(tiles);
        for (int i = 0; i < shuffledTiles.Length - 1; i++)
        {
            int randomIndex = prng.Next(i+1, shuffledTiles.Length);
            Tile temp = shuffledTiles[i];
            shuffledTiles[i] = shuffledTiles[randomIndex];
            shuffledTiles[randomIndex] = temp;
        }
            
        int obstaclesNumber = (int)(map.mapSize.x * map.mapSize.y * map.obstacles);
        int currentObstacleNumber = 0;
        for (int i = 0; i < obstaclesNumber; i++)
        {
            Tile tile = shuffledTiles[i];
            Vector2Int tilePos = tile.tileMapPos;
            Vector3 obstaclePos = tile.transform.position;

            currentObstacleNumber++;
            tiles[tilePos.y, tilePos.x].state = Tile_State.NOT_EMPTY;
            if (MapIsFullyAccesible(tiles, currentObstacleNumber))
            {
                int obstacleHeight = prng.Next(map.obstacleMinHeight, map.obstacleMaxHeight);
                Vector3 offset = new Vector3(0, obstacleHeight/2.0f, 0);
                Transform obstacle = Instantiate(obstaclePrefab, obstaclePos + offset, Quaternion.identity);
                obstacle.localScale = new Vector3(obstacle.localScale.x, obstacleHeight, obstacle.localScale.z);
                Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                obstacleMaterial.color = Color.Lerp(map.bottomColor, map.topColor, ((float)tile.tileMapPos.y + 1.0f) / (float)map.mapSize.y);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                obstacle.parent = mapHolder;
            }
            else
            {
                tiles[tilePos.y, tilePos.x].state = Tile_State.EMPTY;
                currentObstacleNumber--;
            }
        }

        navmeshFloor.localScale = new Vector3(map.maxMapSize.x, map.maxMapSize.y, 1.0f);
        Vector2Int mapDelta = map.maxMapSize /2 - map.mapSize /2;
        Vector3 leftNavmeshPos = transform.position - new Vector3(map.mapSize.x / 2.0f + mapDelta.x / 2.0f, 0.0f, 0.0f);
        GenerateNavMeshMask(leftNavmeshPos, new Vector2Int(mapDelta.x, map.maxMapSize.y), mapHolder);
        Vector3 topNavmeshPos = transform.position + new Vector3(0.0f, 0.0f, map.mapSize.y / 2.0f + mapDelta.y / 2.0f);
        GenerateNavMeshMask(topNavmeshPos, new Vector2Int(map.maxMapSize.x, mapDelta.y), mapHolder);
        Vector3 rightNavmeshPos = transform.position + new Vector3(map.mapSize.x/2.0f + mapDelta.x/2.0f, 0.0f, 0.0f);
        GenerateNavMeshMask(rightNavmeshPos, new Vector2Int(mapDelta.x, map.maxMapSize.y), mapHolder);
        Vector3 bottomNavmeshPos = transform.position - new Vector3(0.0f, 0.0f, map.mapSize.y / 2.0f + mapDelta.y / 2.0f);
        GenerateNavMeshMask(bottomNavmeshPos, new Vector2Int(map.maxMapSize.x, mapDelta.y), mapHolder);
    }

    public Tile GetRandomFreeTile()
    {
        while (true)
        {
            int row = UnityEngine.Random.Range(0, map.mapSize[0]);
            int col = UnityEngine.Random.Range(0, map.mapSize[1]);
            Tile tile = tiles[col, row];
            if (tile.state == Tile_State.EMPTY)
            { 
                return tile; 
            }
        }
    }

    private void GenerateNavMeshMask(Vector3 pos, Vector2Int scale, Transform mapHolder)
    {
        Transform navmeshMask = Instantiate(navmeshMaskPrefab, pos, navmeshMaskPrefab.rotation);
        navmeshMask.localScale = new Vector3(scale.x, navmeshMask.localScale.z, scale.y);
        navmeshMask.parent = mapHolder;
    }

    private bool MapIsFullyAccesible(Tile[,] tiles, int obstacle_count)
    {
        Tile[,] tilesCopy = CopyTiles(tiles);
        Stack<Vector2Int> visitedTiles = new Stack<Vector2Int>();
        int visitedTilesCount = 0;
        for (int col = -1; col < 2; col++)
        {
            for(int row = -1; row < 2; row++)
            {
                ref Tile tile = ref tilesCopy[map.mapSize.y / 2 + col, map.mapSize.x / 2 + row];
                tile.state = Tile_State.VISITED;
                visitedTiles.Push(new Vector2Int(tile.tileMapPos.x, tile.tileMapPos.y));
                visitedTilesCount++;
            }
        }
        
        while (visitedTiles.Count > 0)
        {
            Vector2Int tilePos = visitedTiles.Peek();
            if((tilePos.y + 1) < map.mapSize.y)
            {
                ref Tile topTile = ref tilesCopy[tilePos.y + 1, tilePos.x];
                if (topTile.state == Tile_State.EMPTY)
                {
                    VisitTile(ref visitedTiles, ref topTile, ref visitedTilesCount);
                    continue;
                }
            }
            if((tilePos.x + 1) < map.mapSize.x)
            {
                ref Tile rightTile = ref tilesCopy[tilePos.y, tilePos.x + 1];
                if (rightTile.state == Tile_State.EMPTY)
                {
                    VisitTile(ref visitedTiles, ref rightTile, ref visitedTilesCount);
                    continue;
                }
            }
            if((tilePos.y - 1) >= 0)
            {
                ref Tile bottomTile = ref tilesCopy[tilePos.y - 1, tilePos.x];
                if (bottomTile.state == Tile_State.EMPTY)
                {
                    VisitTile(ref visitedTiles, ref bottomTile, ref visitedTilesCount);
                    continue;
                }
            }
            if((tilePos.x - 1) >= 0)
            {
                ref Tile leftTile = ref tilesCopy[tilePos.y, tilePos.x - 1];
                if (leftTile.state == Tile_State.EMPTY)
                {
                    VisitTile(ref visitedTiles, ref leftTile, ref visitedTilesCount);
                    continue;
                }
            }
            visitedTiles.Pop();
        }
        int totalEmptyTiles = map.mapSize.x * map.mapSize.y - obstacle_count;
        return totalEmptyTiles == visitedTilesCount;
    }

    private Tile[,] CopyTiles(Tile[,] tiles)
    {
        int columns = tiles.GetLength(0);
        int rows = tiles.GetLength(1);
        Tile[,] result = new Tile[columns, rows];
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                result[i, j] = tiles[i, j];
            }
        }
        return result;
    }

    private void VisitTile(ref Stack<Vector2Int> visitedTiles, ref Tile tile, ref int visitedTilesCount)
    {
        tile.state = Tile_State.VISITED;
        visitedTiles.Push(tile.tileMapPos);
        visitedTilesCount++;
    }

    private Tile[] FlattenArray(Tile[,] tiles)
    {
        Tile[] result = new Tile[tiles.Length];
        
        for(int y = 0, i = 0; y < tiles.GetLength(0); y++)
        {
            for(int x = 0; x < tiles.GetLength(1); x++, i++)
            {
                result[i] = tiles[y, x];
            }
        }

        return result;
    }
}