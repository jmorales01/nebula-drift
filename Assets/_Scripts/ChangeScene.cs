
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void GoToSceneTwo()
    {
        SceneManager.LoadScene("GameScene");
    }
}
