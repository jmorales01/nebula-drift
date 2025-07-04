
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void GoToSceneTwo()
    {
        Time.timeScale = 1f; // ← Asegura que el juego corra normalmente
        SceneManager.LoadScene("GameScene");
    }

    public void GoToSceneMenu()
    {
        Time.timeScale = 1f; // ← Igual aquí, por si vas al menú después de un Game Over
        SceneManager.LoadScene("Menu");
    }
}
