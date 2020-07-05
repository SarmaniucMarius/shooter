using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;
    public float spawnTime;
    public Transform player;
    public Transform[] dropPrefabs;

    MapGenerator mapGenerator;
    float nextSpawnTime;
    int currentWaveCount;
    [System.NonSerialized]
    public int killedEnemies = 0;
    int spawnedEnemies;
    string currentDrop = null;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    private void Update()
    {
        if(currentWaveCount < waves.Length)
        {
            Wave currentWave = waves[currentWaveCount];
            if (killedEnemies == currentWave.enemyCount)
            {
                if(currentDrop == null)
                {
                    currentDrop = Instantiate(dropPrefabs[currentWaveCount]).name;
                }
                if(GameObject.Find(currentDrop) == null)
                {
                    currentDrop = null;
                    if (mapGenerator.maps.Length > ++mapGenerator.currentMap)
                    {
                        mapGenerator.GenerateMap();
                        player.position = new Vector3(0, 1, 0);
                    }
                    currentWaveCount++;
                    killedEnemies = 0;
                    spawnedEnemies = 0;
                }
            }
            else
            {
                if ((Time.time >= nextSpawnTime) && (spawnedEnemies != currentWave.enemyCount))
                {
                    StartCoroutine("SpawnEnemy");
                    nextSpawnTime = Time.time + currentWave.enemySpawnTimer;
                    spawnedEnemies++;
                }
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        Tile tile = mapGenerator.GetRandomFreeTile();
        Material tileMaterial = tile.transform.GetComponent<Renderer>().material;
        Color tileOriginalColor = tileMaterial.color;

        float precent = 0;
        while(precent <= 1f)
        {
            precent += Time.deltaTime * (1f / spawnTime);

            tileMaterial.color = Color.Lerp(tileOriginalColor, Color.red, precent);

            yield return null;
        }
       
        tileMaterial.color = tileOriginalColor;
        Enemy newEnemy = Instantiate(enemy, tile.transform.position, Quaternion.identity);
        newEnemy.mySpawner = this;
    }

    [System.Serializable]
    public struct Wave
    {
        public int enemyCount;
        public float enemySpawnTimer;
        public Vector3 worldPos;
    }
}
