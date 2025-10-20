using System;
using UnityEngine;
using TMPro;

public class HUDUIDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    private void Awake()
    {
        if(LevelBuilder.Instance == null) Debug.LogError("LevelBuilder Instance null");

        GameEvents.OnScoreUpdated += UpdateTotalScoreHUD;
        GameEvents.OnStartGame += UpdateLevelLineUIDisplay;
    }

    void Start()
    {
        UpdateLevelLineUIDisplay();
        UpdateTotalScoreHUD();
    }

    private void UpdateLevelLineUIDisplay()
    {
        levelName.text = LevelBuilder.Instance.GetLevelName();
    }

    public void UpdateTotalScoreHUD()
    {
        string score = ScoreManager.Instance.GetCurrentScore().ToString();
        totalScoreText.text = $"Total Score: {score}";
    }
}
