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
        if (antariksaPrefabs == null || antariksaPrefabs.Length == 0)
        {
            Debug.LogError("Array antariksaPrefabs kosong atau null di Inspector!");
            enabled = false; // Nonaktifkan script jika prefab tidak diatur
            return;
        }
        ResetSpawner();
    }

    void Update()
    {
        if (!enabled || GameManager.Instance == null || !GameManager.Instance.GetComponent<GameManager>().isGameActive)
            return;

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
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance tidak ditemukan di SpawnObjekAntariksa!");
            return;
        }

        GameObject prefabToSpawn = antariksaPrefabs[Random.Range(0, antariksaPrefabs.Length)];
        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab antariksa null di antariksaPrefabs!");
            return;
        }

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        GameObject newObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // NormalizeSpriteSize dihapus

        GerakObjekAntariksa objScript = newObj.GetComponent<GerakObjekAntariksa>();
        if (objScript != null)
        {
            objScript.SetSpeed(GameManager.Instance.GetObjectSpeed());
        }
        else
        {
            Debug.LogWarning($"GerakObjekAntariksa tidak ditemukan pada {newObj.name}");
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
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance tidak ditemukan di ResetSpawner!");
            return;
        }

        spawnInterval = GameManager.Instance.GetSpawnInterval();
        timer = 0;
        enabled = true;
    }
}
