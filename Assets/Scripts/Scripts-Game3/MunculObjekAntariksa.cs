using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunculObjekAntariksa : MonoBehaviour
{
    public GameObject[] antariksaPrefabs;
    private float spawnInterval;
    private float timer;
    public float spawnX = 8f;
    public float spawnY = 0f;
    public float minDistance = 4f;

    void Start()
    {
        ResetSpawner();
        if (antariksaPrefabs.Length == 0)
        {
            Debug.LogError("Array antariksaPrefabs kosong. Isi di Inspector.");
        }
    }

    void Update()
    {
        if (!enabled) return;

        timer += Time.deltaTime;
        if (timer > spawnInterval && IsSafeToSpawn())
        {
            SpawnObjekAntariksa();
            timer = 0;
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
        GameObject prefabToSpawn = antariksaPrefabs[Random.Range(0, antariksaPrefabs.Length)];
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

    }

    public void ResetForNewLevel(float newInterval)
    {
        spawnInterval = newInterval;
        timer = 0;
        enabled = true;
    }

    public void ResetSpawner()
    {
        spawnInterval = GameManager.Instance.GetSpawnInterval();
        timer = 0;
        enabled = true;
    }
}
