using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class SpawnManager : MonoBehaviour
{
    public int repairToolsAmount = 3;
    public int terrainsAmount = 60;
    public int buildingsAmount = 5;
    public GameObject player;
    public GameObject[] enemyPrefabs;
    public GameObject repairToolPrefab;
    public GameObject[] terrainPrefabs;
    public GameObject[] buildingPrefabs;

    public float horizontalBound = 60f;
    public float verticalBound = 60f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20;  i++)
        {
            SpawnEnemy();
        }
        SpawnRepairTools(repairToolsAmount);
        StartCoroutine(SpawnTimer());
        SpawnTerrain(terrainsAmount);
        SpawnBuildings(5);
    }

    // Update is called once per frame
    void Update()
    {
        int repairToolsInScene = GameObject.FindGameObjectsWithTag("Repair Tool").Length;

        if (repairToolsInScene < repairToolsAmount) SpawnRepairTools(repairToolsAmount-repairToolsInScene);
    }

    IEnumerator SpawnTimer()
    {
        if (player != null)
        {
            SpawnEnemy();
            yield return new WaitForSecondsRealtime(1);
            StartCoroutine(SpawnTimer());

        }
    }

    void SpawnEnemy()
    {
        // Optimization limits enemies shown, if enemies in scene is already 50, don't spawn
        int enemyAmount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        
        if(enemyAmount < 50)
        {
            int randIndex = GetEnemyIndex();
            int hp;

            if (randIndex == 1) hp = 1;
            else hp = Random.Range(2, 4);

            GameObject clone = Instantiate(enemyPrefabs[randIndex], GetNewSpawnPos(1f), GetRandomRotation(0));

            if(randIndex == 0) clone.transform.position = 
                    new Vector3(clone.transform.position.x, 1f, clone.transform.position.z);
            else if (randIndex == 1) clone.transform.position =
                    new Vector3(clone.transform.position.x, 2.2f, clone.transform.position.z);

            clone.GetComponent<EnemyBehaviour>().hp = hp;
        }
    }

    void SpawnTerrain(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(terrainPrefabs[GetTerrainIndex()], GetNewSpawnPos(0f), GetRandomRotation(0));
        }
    }

    public Vector3 GetNewSpawnPos(float height)
    {
        float randomHorizontalBoundary = Random.Range(-horizontalBound, horizontalBound);
        float randomVerticalBoundary = Random.Range(-verticalBound, verticalBound);

        // Fix to avoid spawning on player spawn
        Vector3 spawnPos = new Vector3(randomHorizontalBoundary, height, randomVerticalBoundary);
        if (randomHorizontalBoundary < 3 && randomHorizontalBoundary > -8 && 
            randomVerticalBoundary > -6 && randomVerticalBoundary < 5f)
        {
            spawnPos = GetNewSpawnPos(height);
        }
        return spawnPos;
    }

    private void SpawnRepairTools(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Instantiate(repairToolPrefab, GetNewSpawnPos(1f), GetRandomRotation(90));
        }
    }

    private void SpawnBuildings(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(buildingPrefabs[GetBuildingIndex()], GetNewSpawnPos(1.5f), GetRandomRotation(0));
        }
    }

    int GetEnemyIndex()
    {
        float random = Random.Range(0f, 100f);

        if (random <= 30f) // Percentage to spawn yippees
        {
            return 1; // Spawns yippees
        }
        else
        {
            return 0; // Spawns normals
        }
    }

    int GetTerrainIndex()
    {
        int random = Random.Range(0, terrainPrefabs.Length);

        return random;
    }

    int GetBuildingIndex()
    {
        int random = Random.Range(0, buildingPrefabs.Length);

        return random;
    }

    Quaternion GetRandomRotation(float xRot)
    {
        float randomAngle = Random.Range(0f, 360f);
        Quaternion random = Quaternion.Euler(xRot,randomAngle,0);
        
        return random;
    }
}
