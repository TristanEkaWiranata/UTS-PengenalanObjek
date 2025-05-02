using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] sampahPrefabs; // Prefab organik dan non-organik
    public float spawnInterval = 2f;   // Waktu antar spawn
    public Transform spawnPoint;       // Titik di kanan layar

    void Start()
    {
        InvokeRepeating("SpawnSampah", 1f, spawnInterval);
    }

    void SpawnSampah()
    {
        int index = Random.Range(0, sampahPrefabs.Length);
        Instantiate(sampahPrefabs[index], spawnPoint.position, Quaternion.identity);
    }
}
