using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunculObjekAntariksa : MonoBehaviour
{
    public GameObject[] antariksaPrefabs; // Prefab Planet dan Spaceship
    private float spawnInterval;          // Interval spawn (disesuaikan oleh LevelManager)
    private float timer;
    private int objectsSpawned = 0;      // Menghitung objek yang sudah di-spawn di level ini
    public float spawnX = 8f;            // Posisi X untuk spawn (kanan layar)
    public float spawnY = 0f;            // Posisi Y tetap untuk spawn
    public float minDistance = 8f;       // Jarak minimum antar objek

    private Vector3 lastSpawnPosition;   // Posisi spawn objek terakhir

    void Start()
    {
        spawnInterval = LevelManager.Instance.GetSpawnInterval();
        lastSpawnPosition = new Vector3(spawnX, spawnY, 0);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            // Cek apakah masih perlu spawn dan jarak aman
            if (objectsSpawned < LevelManager.Instance.GetObjectsToSort() && IsSafeToSpawn())
            {
                SpawnObjekAntariksa();
                timer = 0;
                objectsSpawned++;
            }
        }
    }

    bool IsSafeToSpawn()
    {
        // Cari semua objek antariksa yang masih ada
        GerakObjekAntariksa[] objects = FindObjectsByType<GerakObjekAntariksa>(FindObjectsSortMode.None);
        foreach (var obj in objects)
        {
            float distance = Mathf.Abs(obj.transform.position.x - spawnX);
            if (distance < minDistance)
            {
                return false; // Terlalu dekat, jangan spawn
            }
        }
        return true;
    }

    void SpawnObjekAntariksa()
    {
        int randomIndex = Random.Range(0, antariksaPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        GameObject newObj = Instantiate(antariksaPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        
        lastSpawnPosition = spawnPosition;

        NormalizeSpriteSize normalizer = newObj.GetComponent<NormalizeSpriteSize>();
        if (normalizer != null)
        {
            normalizer.NormalizeSize();
        }

        GerakObjekAntariksa objScript = newObj.GetComponent<GerakObjekAntariksa>();
        if (objScript != null)
        {
            objScript.SetSpeed(LevelManager.Instance.GetObjectSpeed());
        }
    }

    public void ResetForNewLevel(float newInterval)
    {
        spawnInterval = newInterval;
        objectsSpawned = 0;
        timer = 0;
        lastSpawnPosition = new Vector3(spawnX, spawnY, 0);
        enabled = true;
    }
}