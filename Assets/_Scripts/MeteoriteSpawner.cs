using UnityEngine;

public class MeteoriteSpawner : MonoBehaviour
{
    // ¡Cambiado a un array para múltiples prefabs!
    public GameObject[] meteoritePrefabs; 

    public float spawnInterval = 2f;  // Cada cuánto tiempo aparece un meteorito
    public float spawnRangeX = 10f;   // Rango aleatorio en X para la aparición
    public float spawnRangeY = 5f;    // Rango aleatorio en Y para la aparición
    public float spawnZ = 100f;       // Posición Z donde aparecerán los meteoritos (lejos de la nave)

    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnMeteorite();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnMeteorite()
    {
        // Seleccionar un prefab de meteorito aleatoriamente del array
        if (meteoritePrefabs.Length == 0) // Comprobación de seguridad
        {
            Debug.LogWarning("No meteorite prefabs assigned to spawner!");
            return;
        }
        GameObject selectedMeteoritePrefab = meteoritePrefabs[Random.Range(0, meteoritePrefabs.Length)];


        // Calcular una posición aleatoria dentro de los rangos definidos
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);

        Vector3 spawnPosition = new Vector3(randomX, randomY, spawnZ);

        // Instanciar (crear) el meteorito en la escena
        // Usamos el prefab seleccionado aleatoriamente
        Instantiate(selectedMeteoritePrefab, spawnPosition, Quaternion.identity); 
    }
}