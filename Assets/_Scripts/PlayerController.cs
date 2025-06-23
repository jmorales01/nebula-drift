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
    [Header("Audio Settings")]
    public AudioClip laserShotSFX; // Sonido de disparo
    private AudioSource playerAudioSource; // Fuente de audio para el jugador

    private Rigidbody rb;
    private PlayerInputActions playerInputActions; // Referencia a nuestro Input Action Asset
    private Vector2 moveInput; // Almacena el valor de la entrada de movimiento (Vector2)

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        // Suscribirse a los eventos usando métodos nombrados
        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
    }

    void OnEnable()
    {
        // Habilitar el Action Map 'Player' cuando el script se habilita
        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        // Deshabilitar el Action Map 'Player' cuando el script se deshabilita
        playerInputActions.Player.Disable();
        // Desuscribirse de los eventos usando métodos nombrados para evitar fugas de memoria
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

        // Obtener o añadir AudioSource
        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>();
        }
        playerAudioSource.playOnAwake = false; // No reproducir al inicio
    }

    void FixedUpdate()
    {
        // Calcular el movimiento lateral y vertical basado en el input
        Vector3 dodgeMovement = new Vector3(moveInput.x, moveInput.y, 0) * dodgeSpeed * Time.fixedDeltaTime;

        // Calcular el movimiento de avance constante
        Vector3 forwardMovement = Vector3.forward * moveSpeed * Time.fixedDeltaTime;

        // Combinar ambos movimientos para la nueva posición
        Vector3 totalMovement = dodgeMovement + forwardMovement;

        // Mover la nave directamente usando su transform.position
        transform.position += totalMovement;

        // Restringir la posición de la nave dentro de los límites de la pantalla
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -xLimit, xLimit);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -yLimit, yLimit); // ¡Corrección de tipografía aquí!
        transform.position = clampedPosition;
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

            // Reproducir el sonido del láser
            if (playerAudioSource != null && laserShotSFX != null)
            {
                playerAudioSource.PlayOneShot(laserShotSFX); // PlayOneShot es bueno para sonidos que se repiten
            }
        }
        else
        {
            Debug.LogWarning("Laser Prefab or Laser Spawn Point not assigned in PlayerController!");
        }
    }
}