using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("Meteorite Settings")]
    public float speed = 20f; // Velocidad a la que el meteorito se moverá hacia la nave
    public float lifeTime = 10f; // Tiempo antes de que el meteorito se destruya si no colisiona
    public int scoreValue = 10; // Puntos que da este meteorito al ser destruido

    [Header("FX Settings")]
    public GameObject explosionFXPrefab; // Prefab del efecto de explosión
    public AudioClip explosionSFX;      // Sonido de explosión

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Aplicar velocidad al Rigidbody (ya que no es cinemático)
            // Se mueve hacia el eje Z negativo (hacia la nave)
            rb.linearVelocity = Vector3.back * speed; // Vector3.back es shorthand para (0, 0, -1)
        }
        else
        {
            Debug.LogError("Rigidbody component not found on Meteorite! Please add one.", this);
            enabled = false;
        }

        // Destruir el meteorito después de un tiempo si no ha chocado o salido de la vista
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Si choca con la nave del jugador
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Impacto de meteorito en la nave!");
            // Informar al GameManager que la nave ha perdido una vida
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
            // Instanciar FX y reproducir sonido (puedes añadirlo aquí si quieres la explosión al chocar con el jugador)
            InstantiateFXAndSFX(); // Llama a la función para instanciar el FX y SFX
            Destroy(gameObject); // Destruye el meteorito
        }
        // Si choca con un laser
        else if (other.CompareTag("Laser"))
        {
            Debug.Log("¡Meteorito impactado por láser!");
            // Informar al GameManager que el jugador ha ganado puntos
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }
            InstantiateFXAndSFX(); // Llama a la función para instanciar el FX y SFX
            Destroy(other.gameObject); // Destruye el láser (el objeto con el que chocó)
            Destroy(gameObject); // Destruye el meteorito
        }
    }

    // Nuevo método para instanciar efectos visuales y de sonido
    void InstantiateFXAndSFX()
    {
        if (explosionFXPrefab != null)
        {
            Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSFX != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position); // Reproduce el sonido en la posición del meteorito
        }
    }
}