using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] GameObject mainMenuPanel;
    
    [Header("HUD Panel")]
    [SerializeField] GameObject hudPanel;
    
    [Header("EndGamePanel")]
    [SerializeField] GameObject endGamePanel;
    [SerializeField] TextMeshProUGUI endGameText;
    [SerializeField] Button endGameButton;
    [SerializeField] TextMeshProUGUI endGameButtonText;

    [Header("Results Panel")]
    [SerializeField] private TextMeshProUGUI levelScoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI totalDeathsText;
    
    [Header("Game Beated Panel")]
    [SerializeField] GameObject beatedGamePanel;
    [SerializeField] TextMeshProUGUI beatedGameText;
    [SerializeField] Button beatedGameButton;
    [SerializeField] TextMeshProUGUI beatedGameButtonText;
    [SerializeField] private TextMeshProUGUI beatedTotalScoreText;
    [SerializeField] private TextMeshProUGUI beatedTotalDeathsText;

    private void Awake()
    {
        GameEvents.OnStartGame += OnStartGameUI;
        GameEvents.OnPlayerDie += OnPlayerDie;
        GameEvents.OnLevelComplete += OnLevelComplete;
        GameEvents.OnScoreUpdated += UpdateScoreUI;
        GameEvents.OnGameOver += OnGameEnd;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDie -= OnPlayerDie;
        GameEvents.OnLevelComplete -= OnLevelComplete;
        GameEvents.OnScoreUpdated -= UpdateScoreUI;
        GameEvents.OnGameOver -= OnGameEnd;
    }

    public void UpdateScoreUI()
    {
        levelScoreText.text = "Level Completed Points: +" + LevelBuilder.Instance.GetBaseScore();
        totalScoreText.text = "Total Score: " + ScoreManager.Instance.GetCurrentScore();
        totalDeathsText.text = "Deaths Count: " + ScoreManager.Instance.GetDeathCount();
    }

    public void OnPlayerDie()
    {
        endGamePanel.SetActive(true);
        ToogleLevelHUD(false);
        
        // Seleciona o botão como o primeiro
        EventSystem.current.SetSelectedGameObject(null); // limpa seleção anterior
        EventSystem.current.SetSelectedGameObject(endGameButton.gameObject);

        endGameText.text = "Ops, you died!";
        endGameButtonText.text = "Try again";
        levelScoreText.enabled = false;
        
        endGameButton.onClick.RemoveAllListeners();
        endGameButton.onClick.AddListener(() =>
        {
            endGamePanel.SetActive(false);
            GameManager.Instance.RestartLevel();
            ToogleLevelHUD(true);
        });
    }

    public void OnLevelComplete()
    {
        endGamePanel.SetActive(true);
        ToogleLevelHUD(false);
        
        // Seleciona o botão como o primeiro
        EventSystem.current.SetSelectedGameObject(null); // limpa seleção anterior
        EventSystem.current.SetSelectedGameObject(endGameButton.gameObject);
        
        endGameText.text = "Level Completed";
        endGameButtonText.text = "Next Level";
        levelScoreText.enabled = true;
        
        endGameButton.onClick.RemoveAllListeners();
        endGameButton.onClick.AddListener(() =>
        {
            endGamePanel.SetActive(false);
            GameManager.Instance.LoadNextLevel();
            ToogleLevelHUD(true);
        });
    }

    public void OnGameEnd()
    {
        beatedGamePanel.SetActive(true);
        ToogleLevelHUD(false);
        
        // Seleciona o botão como o primeiro
        EventSystem.current.SetSelectedGameObject(null); // limpa seleção anterior
        EventSystem.current.SetSelectedGameObject(beatedGameButton.gameObject);
        
        beatedGameText.text = "Congratulations You escaped!";
        beatedGameButtonText.text = "Finish";
        
        beatedTotalScoreText.text = ScoreManager.Instance.GetCurrentScore().ToString();
        beatedTotalDeathsText.text = ScoreManager.Instance.GetDeathCount().ToString();
        
        beatedGameButton.onClick.RemoveAllListeners();
        beatedGameButton.onClick.AddListener(() =>
        {
            beatedGamePanel.SetActive(false);
            GameManager.Instance.HomeButton();
            mainMenuPanel.SetActive(true);
            ToogleLevelHUD(false);
            GameManager.Instance.ClearStateSave();
            SaveManager.Instance.SaveData();
        });
    }

    public void ToogleLevelHUD(bool isActive)
    {
        hudPanel.SetActive(isActive);
    }
    
    private void OnStartGameUI()
    {
        ToogleLevelHUD(true);
    }
}
