using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 2f; 

    private Rigidbody rb;

    private AudioSource audioSource; // Referencia al componente AudioSource para el sonido de disparo
    public AudioClip laserShotClip;  // AudioClip para el sonido de disparo del láser

    // AÑADE ESTA LÍNEA para el sonido de impacto
    public AudioClip impactSoundClip; // AudioClip para el sonido cuando impacta un meteorito

    // --- NUEVAS VARIABLES DE VOLUMEN ---
    public float laserShotVolume = 0.5f; // Volumen para el disparo del láser (0.0 a 1.0)
    public float impactSoundVolume = 1.0f; // Volumen para el sonido de impacto (0.0 a 1.0)
    // -----------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on Laser Prefab. Adding one.", this);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = laserShotClip; // Asigna el clip de disparo al AudioSource
        audioSource.playOnAwake = false;

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rigidbody component not found on Laser! Please add one.", this);
            enabled = false;
        }

        // Reproduce el sonido de disparo del láser cuando se crea
        if (audioSource != null && laserShotClip != null)
        {
            // --- APLICA EL VOLUMEN AL SONIDO DE DISPARO ---
            audioSource.PlayOneShot(laserShotClip, laserShotVolume); 
        }

        Destroy(gameObject, lifeTime);
    }

    // Método para detectar colisiones cuando Is Trigger está activado en el Collider del láser
    void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que el otro objeto tenga un Rigidbody para que OnTriggerEnter funcione correctamente.
        // Y que los meteoritos tengan el Tag "Meteorito"
        if (other.CompareTag("Meteorito"))
        {
            // AÑADE ESTAS LÍNEAS para el sonido de impacto
            if (impactSoundClip != null)
            {
                // --- APLICA EL VOLUMEN AL SONIDO DE IMPACTO ---
                AudioSource.PlayClipAtPoint(impactSoundClip, transform.position, impactSoundVolume);
            }

            // Aquí puedes añadir la lógica para dañar el meteorito o destruirlo
            // Por ahora, solo destruiremos el propio láser al impactar.
            Destroy(gameObject); // Destruye el láser después de impactar
        }
    }
}