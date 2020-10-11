using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawnManager : MonoBehaviour
{
    public static ArrowSpawnManager Instance;
    [SerializeField] private GameObject[] arrowPrefabs;
    private float spawnPosUpX = -2.84f;
    private float spawnPosRightX = -2.4f;
    private float spawnPosLeftX = -1.96f;
    private float spawnPosDownX = -1.52f;
    private float spawnPosY = 1.265f;

    [SerializeField] private float spawnDelay = 1;
    [SerializeField] private float spawnInterval = 1;
    
    void Start()
    {
        Instance = this;
        InvokeRepeating("SpawnRandomArrow", spawnDelay, spawnInterval);
    }

    private void SpawnRandomArrow()
    {
        if (GameManager.gameManager.stopSpawn == false)
        {
            int randomIndex = Random.Range(0, 5);
            int upIndex = Random.Range(0, 3);
            int rightIndex = Random.Range(3, 6);
            int leftIndex = Random.Range(6, 9);
            int downIndex = Random.Range(9, 12);
            Vector3 spawnPos;

            if (randomIndex == 0)
            {
                spawnPos = new Vector3(spawnPosUpX, spawnPosY, 0);
                Instantiate(arrowPrefabs[upIndex], spawnPos, arrowPrefabs[upIndex].transform.rotation);
            }
            else if (randomIndex == 1)
            {
                spawnPos = new Vector3(spawnPosRightX, spawnPosY, 0);
                Instantiate(arrowPrefabs[rightIndex], spawnPos, arrowPrefabs[rightIndex].transform.rotation);
            }
            else if (randomIndex == 2)
            {
                spawnPos = new Vector3(spawnPosLeftX, spawnPosY, 0);
                Instantiate(arrowPrefabs[leftIndex], spawnPos, arrowPrefabs[leftIndex].transform.rotation);
            }
            else
            {
                spawnPos = new Vector3(spawnPosDownX, spawnPosY, 0);
                Instantiate(arrowPrefabs[downIndex], spawnPos, arrowPrefabs[downIndex].transform.rotation);
            }
        }
    }
}
