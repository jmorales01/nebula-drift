using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("Meteorite Settings")]
    public float speed = 20f; // Velocidad a la que el meteorito se moverá hacia la nave
    public float lifeTime = 10f; // Tiempo antes de que el meteorito se destruya si no colisiona
    public int scoreValue = 10; // Puntos que da este meteorito al ser destruido

    [Header("FX Settings")]
    public GameObject explosionFXPrefab; // Prefab del efecto de explosión del meteorito
    public AudioClip explosionSFX;      // Sonido de explosión del meteorito

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
            enabled = false; // Deshabilita el script si no hay Rigidbody
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

            // Intentamos obtener el PlayerController del objeto que colisionó
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Si encontramos el PlayerController, le decimos que tome daño.
                // El PlayerController será el encargado de notificar al GameManager y de los efectos de la nave.
                player.TakeDamage(); 
            }
            
            // Instanciar FX y reproducir sonido para el meteorito que explota
            InstantiateFXAndSFX(); 
            
            Destroy(gameObject); // Destruye el meteorito
        }
        // Si choca con un laser (asumiendo que tu láser tiene el tag "Laser")
        else if (other.CompareTag("Laser"))
        {
            Debug.Log("¡Meteorito impactado por láser!");

            // Informar al GameManager que el jugador ha ganado puntos
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }

            // Instanciar FX y reproducir sonido para el meteorito que explota
            InstantiateFXAndSFX(); 
            
            Destroy(other.gameObject); // Destruye el láser (el objeto con el que chocó)
            Destroy(gameObject);       // Destruye el meteorito
        }
    }

    // Nuevo método para instanciar efectos visuales y de sonido del meteorito
    void InstantiateFXAndSFX()
    {
        if (explosionFXPrefab != null)
        {
            // Instancia el prefab de explosión en la posición del meteorito
            Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSFX != null)
        {
            // Reproduce el sonido de explosión en la posición del meteorito
            // AudioSource.PlayClipAtPoint es bueno para sonidos one-shot que no necesitan un AudioSource persistente
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position); 
        }
    }
}