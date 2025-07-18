using UnityEngine;

public class MeteoriteSpawner : MonoBehaviour
{
    public GameObject[] meteoritePrefabs; 

    [Header("Spawn Settings")]
    public float initialSpawnInterval = 2f; 
    public float minSpawnInterval = 0.5f;   
    public float intervalDecreaseRate = 0.05f; 
    public float spawnRangeX = 10f; 
    public float spawnRangeY = 5f; 
    public float spawnZ = 100f; 

    [Header("Meteorite Size Settings")]
    public float minMeteoriteScale = 0.5f; // Escala mínima (0.5 = mitad del tamaño original)
    public float maxMeteoriteScale = 2.0f; // Escala máxima (2.0 = doble del tamaño original)

    private float currentSpawnInterval;
    private float nextSpawnTime;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
    }

    void Update()
    {
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - Time.deltaTime * intervalDecreaseRate);

        if (Time.time >= nextSpawnTime)
        {
            SpawnMeteorite();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void SpawnMeteorite()
    {
        if (meteoritePrefabs.Length == 0)
        {
            Debug.LogWarning("No meteorite prefabs assigned to spawner!");
            return;
        }
        GameObject selectedMeteoritePrefab = meteoritePrefabs[Random.Range(0, meteoritePrefabs.Length)];

        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);

        Vector3 spawnPosition = new Vector3(randomX, randomY, spawnZ);

        // Instanciar el meteorito
        GameObject newMeteorite = Instantiate(selectedMeteoritePrefab, spawnPosition, Quaternion.identity); 
        
        // Aplicar una escala aleatoria
        float randomScale = Random.Range(minMeteoriteScale, maxMeteoriteScale);
        newMeteorite.transform.localScale = Vector3.one * randomScale; // Vector3.one aplica la misma escala en X, Y, Z
    }
}