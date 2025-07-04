using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        if (HighScoreManager.Instance != null && HighScoreManager.Instance.highScores.scores.Count > 0)
        {
            Debug.Log("Cargando puntajes...");
            string text = "Top 10 Puntajes:\n";
            int rank = 1;

            foreach (var entry in HighScoreManager.Instance.highScores.scores)
            {
                Debug.Log("Puntaje: " + entry.score); // ← ¿esto aparece en consola?
                text += $"{rank}. {entry.score}\n";
                rank++;
            }

            highScoreText.text = text;
        }
        else
        {
            highScoreText.text = "No hay puntajes aún.";
            Debug.LogWarning("HighScoreManager.Instance es null");
        }
    }
}
