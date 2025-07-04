using UnityEngine;
using UnityEngine.InputSystem; // ¡Nueva librería para el Input System!

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public GameObject laserPrefab; // Referencia al prefab del láser
    public Transform laserSpawnPoint; // Punto desde donde se disparará el láser
    public float fireRate = 0.5f; // Cadencia de disparo (segundos entre disparos)

    private float nextFireTime; // Para controlar la cadencia
    public float moveSpeed = 10f; // Velocidad de avance constante
    public float dodgeSpeed = 7f; // Velocidad de esquivar (movimiento lateral/vertical)
    public float xLimit = 8f;     // Límite horizontal de la pantalla
    public float yLimit = 4f;     // Límite vertical de la pantalla

    // --- NUEVAS VARIABLES PARA LA INCLINACIÓN ---
    [Header("Tilt Settings")]
    public float tiltAmount = 20f; // Grados máximos de inclinación
    public float tiltSmoothness = 5f; // Velocidad de la inclinación (mayor valor = más rápido)
    // --- FIN NUEVAS VARIABLES PARA LA INCLINACIÓN ---

    [Header("Audio Settings")]
    public AudioClip laserShotSFX; // Sonido de disparo
    public AudioClip playerHitSFX; // Sonido cuando el jugador es golpeado
    public GameObject playerExplosionFXPrefab; // Prefab del efecto de explosión del jugador (pequeña o grande)

    private AudioSource playerAudioSource; // Fuente de audio para el jugador

    private Rigidbody rb;
    private PlayerInputActions playerInputActions; // Referencia a nuestro Input Action Asset
    private Vector2 moveInput; // Almacena el valor de la entrada de movimiento (Vector2)

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
    }

    void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Player.Disable();
        playerInputActions.Player.Move.performed -= OnMovePerformed;
        playerInputActions.Player.Move.canceled -= OnMoveCanceled;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on PlayerShip!");
        }

        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>();
        }
        playerAudioSource.playOnAwake = false;
    }

    void FixedUpdate()
    {
        // Calcular el movimiento lateral y vertical basado en el input
        Vector3 dodgeMovement = new Vector3(moveInput.x, moveInput.y, 0) * dodgeSpeed * Time.fixedDeltaTime;

        // Calcular el movimiento de avance constante (parece que la nave avanza por sí misma)
        Vector3 forwardMovement = Vector3.forward * moveSpeed * Time.fixedDeltaTime;

        // Combinar ambos movimientos para la nueva posición
        Vector3 totalMovement = dodgeMovement + forwardMovement;

        // Mover la nave directamente usando su transform.position
        transform.position += totalMovement;

        // Restringir la posición de la nave dentro de los límites de la pantalla
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -xLimit, xLimit);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -yLimit, yLimit); 
        transform.position = clampedPosition;

        // --- LÓGICA DE INCLINACIÓN DE LA NAVE ---
        // Calcula la rotación objetivo.
        // Eje Z (roll/bank) para el movimiento horizontal.
        // Eje X (pitch) para el movimiento vertical.
        // El movimiento hacia adelante (eje Y en moveInput) no debería inclinar la nave en este tipo de juego.
        Quaternion targetRotation = Quaternion.Euler(
            -moveInput.y * tiltAmount, // Inclinación en X (pitch): Mover hacia arriba inclina hacia abajo (negativo), hacia abajo inclina hacia arriba (positivo). Ajusta el signo si tu modelo se inclina al revés.
            moveInput.x * tiltAmount,  // Inclinación en Y (yaw): Si quieres que rote hacia los lados ligeramente. (Opcional, puede no ser deseado en un juego de naves clásico)
            -moveInput.x * tiltAmount  // Inclinación en Z (roll/bank): Mover a la derecha inclina a la derecha (negativo), a la izquierda inclina a la izquierda (positivo). Ajusta el signo si tu modelo se inclina al revés.
        );

        // Suaviza la transición hacia la rotación objetivo
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * tiltSmoothness);
        // --- FIN LÓGICA DE INCLINACIÓN DE LA NAVE ---
    }

    // Métodos para manejar los eventos de entrada
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    void Update()
    {
        // Disparar si el jugador presiona el botón de disparo y ha pasado el tiempo de fireRate
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= nextFireTime) // Usa la tecla Space del Input System
        {
            ShootLaser();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootLaser()
    {
        if (laserPrefab != null && laserSpawnPoint != null)
        {
            Instantiate(laserPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);

            // Reproducir el sonido del láser (si el AudioClip está asignado en PlayerController)
            // (Si ya lo tienes en el script del Laser, esta parte se puede omitir o usar para un sonido adicional)
            if (playerAudioSource != null && laserShotSFX != null)
            {
                playerAudioSource.PlayOneShot(laserShotSFX);
            }
        }
        else
        {
            Debug.LogWarning("Laser Prefab or Laser Spawn Point not assigned in PlayerController!");
        }
    }

    public void TakeDamage()
    {
        // Reproducir sonido de impacto
        if (playerAudioSource != null && playerHitSFX != null)
        {
            playerAudioSource.PlayOneShot(playerHitSFX);
        }

        // Instanciar efecto visual de daño (ej. una pequeña explosión o chispas)
        if (playerExplosionFXPrefab != null)
        {
            Instantiate(playerExplosionFXPrefab, transform.position, Quaternion.identity);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife();
        }
    }
}