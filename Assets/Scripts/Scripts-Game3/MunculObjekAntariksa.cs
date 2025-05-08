using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunculObjekAntariksa : MonoBehaviour
{
    public GameObject[] antariksaPrefabs;
    private float spawnInterval;
    private float timer;
    private int objectsSpawned = 0;
    public float spawnX = 8f;
    public float spawnY = 0f;
    public float minDistance = 4f;
    private string lastCategory = "";

    void Start()
    {
        ResetSpawner();
        if (antariksaPrefabs.Length == 0)
        {
            Debug.LogError("Array antariksaPrefabs kosong. Isi di Inspector.");
        }
        foreach (var prefab in antariksaPrefabs)
        {
            GerakObjekAntariksa script = prefab.GetComponent<GerakObjekAntariksa>();
            if (script == null || (script.category != "Celestial" && script.category != "ManMade"))
            {
                Debug.LogError($"Prefab {prefab.name} tidak memiliki GerakObjekAntariksa atau category tidak valid.");
            }
        }
        Debug.Log($"Spawner diinisialisasi: {antariksaPrefabs.Length} prefab, spawnX={spawnX}, spawnY={spawnY}, minDistance={minDistance}");
    }

    void Update()
    {
        if (!enabled)
        {
            Debug.LogWarning("Spawner nonaktif, tidak bisa spawn objek.");
            return;
        }
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            if (objectsSpawned < GameManager.Instance.GetObjectsToSort() && IsSafeToSpawn())
            {
                SpawnObjekAntariksa();
                timer = 0;
                objectsSpawned++;
                Debug.Log($"Spawned object #{objectsSpawned}/{GameManager.Instance.GetObjectsToSort()}");
            }
            else
            {
                Debug.Log($"Tidak bisa spawn: objectsSpawned={objectsSpawned}, max={GameManager.Instance.GetObjectsToSort()}, safeToSpawn={IsSafeToSpawn()}");
            }
        }
    }

    bool IsSafeToSpawn()
    {
        GerakObjekAntariksa[] objects = FindObjectsByType<GerakObjekAntariksa>(FindObjectsSortMode.None);
        foreach (var obj in objects)
        {
            if (obj != null && Mathf.Abs(obj.transform.position.x - spawnX) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnObjekAntariksa()
    {
        string nextCategory = (lastCategory == "Celestial") ? "ManMade" : "Celestial";
        List<GameObject> validPrefabs = new List<GameObject>();
        foreach (var prefab in antariksaPrefabs)
        {
            GerakObjekAntariksa script = prefab.GetComponent<GerakObjekAntariksa>();
            if (script != null && script.category == nextCategory)
            {
                validPrefabs.Add(prefab);
            }
        }

        if (validPrefabs.Count == 0)
        {
            Debug.LogWarning($"Tidak ada prefab untuk {nextCategory}. Menggunakan kategori lain.");
            nextCategory = (nextCategory == "Celestial") ? "ManMade" : "Celestial";
            foreach (var prefab in antariksaPrefabs)
            {
                GerakObjekAntariksa script = prefab.GetComponent<GerakObjekAntariksa>();
                if (script != null && script.category == nextCategory)
                {
                    validPrefabs.Add(prefab);
                }
            }
        }

        GameObject prefabToSpawn = validPrefabs[Random.Range(0, validPrefabs.Count)];
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        GameObject newObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        NormalizeSpriteSize normalizer = newObj.GetComponent<NormalizeSpriteSize>();
        if (normalizer != null)
        {
            normalizer.NormalizeSize();
        }

        GerakObjekAntariksa objScript = newObj.GetComponent<GerakObjekAntariksa>();
        if (objScript != null)
        {
            objScript.SetSpeed(GameManager.Instance.GetObjectSpeed());
        }

        lastCategory = nextCategory;
        Debug.Log($"Objek {newObj.name} di-spawn (Kategori: {nextCategory}), Jarak: {GameManager.Instance.GetObjectSpeed() * spawnInterval}");
    }

    public void ResetForNewLevel(float newInterval)
    {
        spawnInterval = newInterval;
        objectsSpawned = 0;
        timer = 0;
        lastCategory = "";
        enabled = true;
        Debug.Log($"Spawner direset untuk level baru: interval={newInterval}");
    }

    public void ResetSpawner()
    {
        spawnInterval = GameManager.Instance.GetSpawnInterval();
        objectsSpawned = 0;
        timer = 0;
        lastCategory = "";
        enabled = true;
        Debug.Log($"Spawner direset sepenuhnya: interval={spawnInterval}, objectsSpawned={objectsSpawned}, enabled={enabled}");
    }
}