using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Pontuação")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] private int deathCount = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameEvents.OnLevelComplete += EndLevelScore;
        GameEvents.OnPlayerDie += OnPlayerDieUpdateScore;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelComplete -= EndLevelScore;
        GameEvents.OnPlayerDie -= OnPlayerDieUpdateScore;
    }
    
    public void ResetScore()
    {
        totalScore = 0;
        deathCount = 0;
    }
    
    public void AddPoints(int amount)
    {
        totalScore += amount;
    }
    
    public void EndLevelScore()
    {
        AddPoints(LevelBuilder.Instance.GetBaseScore());
        GameEvents.UpdateScore();
        SaveScore();
    }

    public void OnPlayerDieUpdateScore()
    {
        deathCount++;
        totalScore--;
        GameEvents.UpdateScore();
        SaveScore();
    }

    private void SaveScore()
    {
        SaveManager.Instance.TotalDeathsToSave(deathCount);
        SaveManager.Instance.TotalScoreToSave(totalScore);
    }

    public void LoadSavedScore()
    {
        ResetScore();

        totalScore = SaveManager.Instance.Data.totalScore;
        deathCount = SaveManager.Instance.Data.totalDeaths;
        GameEvents.UpdateScore();
    }
    
    public int GetDeathCount() => deathCount;

    public int GetCurrentScore() => totalScore;
}