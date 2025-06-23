using UnityEngine;
using TMPro; // Necesitas esto para interactuar con TextMeshPro

public class GameManager : MonoBehaviour
{
    // Singleton para acceder fácilmente al GameManager desde otros scripts
    public static GameManager Instance { get; private set; }

    [Header("Score Settings")]
    public TextMeshProUGUI scoreText; // Referencia al componente TextMeshPro para mostrar la puntuación
    private int score = 0;

    [Header("Lives Settings")]
    public int maxLives = 3; // Número máximo de vidas
    public TextMeshProUGUI livesText; // Referencia al componente TextMeshPro para mostrar las vidas
    private int currentLives;

    [Header("Game Over Settings")]
    public GameObject gameOverPanel; // Panel de Game Over (UI) que se activará
    public TextMeshProUGUI finalScoreText; // Texto para mostrar la puntuación final en el panel de Game Over

    void Awake()
    {
        // Implementación del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: para que el GameManager persista entre escenas si las tuvieras
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        currentLives = maxLives;
        score = 0;
        UpdateScoreDisplay();
        UpdateLivesDisplay();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Asegurarse de que el panel de Game Over esté oculto al inicio
        }
    }

    // --- Puntuación ---
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreDisplay();
        Debug.Log("Puntuación: " + score);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score;
        }
    }

    // --- Vidas ---
    public void LoseLife()
    {
        currentLives--;
        UpdateLivesDisplay();
        Debug.Log("Vidas restantes: " + currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    void UpdateLivesDisplay()
    {
        if (livesText != null)
        {
            livesText.text = "Vidas: " + currentLives;
        }
    }

    // --- Fin del Juego ---
    void GameOver()
    {
        Debug.Log("¡Juego Terminado!");
        // Aquí puedes detener el juego, mostrar un panel de Game Over, etc.
        Time.timeScale = 0f; // Detener el tiempo del juego

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Puntuación Final: " + score;
            }
        }

        // Desactivar el control del jugador (opcional, podrías tener una pantalla de Game Over con botones)
        // Encuentra el PlayerController y desactívalo. Asegúrate de que tu PlayerController sea el único en la escena
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = false; // Desactiva el script de control del jugador
        }
        // Podrías también desactivar el spawner de meteoritos
        MeteoriteSpawner spawner = FindObjectOfType<MeteoriteSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }
    }

    // Para reiniciar el juego (por ejemplo, desde un botón en el panel de Game Over)
    public void RestartGame()
    {
        Time.timeScale = 1f; // Reanudar el tiempo
        // Cargar la escena actual para reiniciar todo
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}