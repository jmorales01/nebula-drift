using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class HighScoreEntry
{
    public int score;
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> scores = new List<HighScoreEntry>();
}

public class HighScoreManager : MonoBehaviour
{
    private const string PlayerPrefsKey = "HighScores";
    public static HighScoreManager Instance;

    public HighScoreList highScores = new HighScoreList();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
            LoadHighScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        Debug.Log("Guardando puntaje: " + score); // ← Verifica si esto aparece
        highScores.scores.Add(new HighScoreEntry { score = score });
        // Ordenar descendente y limitar a 10
        highScores.scores = highScores.scores.OrderByDescending(s => s.score).Take(10).ToList();
        SaveHighScores();
    }

    void SaveHighScores()
    {
        string json = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    void LoadHighScores()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            highScores = JsonUtility.FromJson<HighScoreList>(json);
        }
    }
}
