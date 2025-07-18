using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("Meteorite Settings")]
    public float baseSpeed = 20f; 
    public float erraticMovementMagnitude = 2f; 
    public float erraticRotationMagnitude = 50f;
    // public float lifeTime = 10f; // <--- Puedes eliminar o comentar esta línea
    public int baseScoreValue = 10; 

    // NUEVO: Posición Z a partir de la cual el meteorito debe ser destruido
    public float destroyZPosition = -20f; // Ajusta este valor según la posición de tu nave y la profundidad de tu escenario

    [Header("FX Settings")]
    public GameObject explosionFXPrefab; 
    public AudioClip explosionSFX; 

    private Rigidbody rb;
    private float currentSpeed; 
    private int currentScoreValue;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        float actualScale = transform.localScale.x; 

        currentSpeed = baseSpeed / actualScale; 
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed * 0.5f, baseSpeed * 2.0f); 

        currentScoreValue = (int)(baseScoreValue * actualScale);
        currentScoreValue = Mathf.Max(1, currentScoreValue);

        if (rb != null)
        {
            float randomX = Random.Range(-erraticMovementMagnitude, erraticMovementMagnitude);
            float randomY = Random.Range(-erraticMovementMagnitude, erraticMovementMagnitude);

            Vector3 initialVelocity = (Vector3.back * currentSpeed) + new Vector3(randomX, randomY, 0f);
            rb.linearVelocity = initialVelocity;

            float randomAngularX = Random.Range(-erraticRotationMagnitude, erraticRotationMagnitude);
            float randomAngularY = Random.Range(-erraticRotationMagnitude, erraticRotationMagnitude);
            float randomAngularZ = Random.Range(-erraticRotationMagnitude, erraticRotationMagnitude);
            rb.angularVelocity = new Vector3(randomAngularX, randomAngularY, randomAngularZ);
        }
        else
        {
            Debug.LogError("Rigidbody component not found on Meteorite! Please add one.", this);
            enabled = false; 
        }

        // ELIMINA O COMENTA ESTA LÍNEA:
        // Destroy(gameObject, lifeTime); 
    }

    // AÑADE ESTE MÉTODO Update() o MODIFÍCALO si ya existe
    void Update()
    {
        // Si el meteorito se ha movido más allá de la posición Z de destrucción
        if (transform.position.z < destroyZPosition)
        {
            // Debug.Log("Meteorito fuera de límites, destruyendo."); // Puedes descomentar para depuración
            Destroy(gameObject); // Destruye el meteorito
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Impacto de meteorito en la nave!");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(); 
            }
            InstantiateFXAndSFX(); 
            Destroy(gameObject); 
        }
        else if (other.CompareTag("Laser"))
        {
            Debug.Log("¡Meteorito impactado por láser!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(currentScoreValue); 
            }
            InstantiateFXAndSFX(); 
            Destroy(other.gameObject); 
            Destroy(gameObject); 
        }
    }

    void InstantiateFXAndSFX()
    {
        if (explosionFXPrefab != null)
        {
            GameObject explosion = Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = transform.localScale; 
        }
        if (explosionSFX != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position); 
        }
    }
}