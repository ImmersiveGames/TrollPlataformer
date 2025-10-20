using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referências")]
    [SerializeField] private CanvasGroup transitionCanvas;
    [SerializeField] private PlayerPool playerPool;
    [SerializeField] private LevelBuilder levelBuilder;

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
    }

    private void Start()
    {
        HomeButton();
    }

    public void StartLevel(int levelIndex)
    {
        StartCoroutine(LevelFlowRoutine(levelIndex));
    }

    private IEnumerator LevelFlowRoutine(int index)
    {
        ShowTransition(true);
        
        LevelBuilder.Instance.TooglelevelMenu(false);
        playerPool.StorePlayer();
        levelBuilder.LoadLevel(index);
        yield return new WaitForSeconds(1f);

        GameEvents.PlayerRespawn();

        yield return new WaitForSeconds(0.3f);
        ShowTransition(false);
        
        GameEvents.StartGame(); // dispara evento de início
        GameEvents.UpdateScore();
    }

    public void OnPlayerDeath()
    {
        GameEvents.PlayerDie(); // dispara evento de morte
        GameEvents.UpdateScore(); // atualiza score (exemplo)
    }

    private IEnumerator RestartLevelRoutine()
    {
        ShowTransition(true);
        yield return new WaitForSeconds(1f);
        
        levelBuilder.ReloadCurrentLevel();
        GameEvents.PlayerRespawn(); // evento de respawn

        ShowTransition(false);
        GameEvents.StartGame(); // dispara evento de reinício
    }

    private IEnumerator ClearLevelRoutine()
    {
        ShowTransition(true);
        yield return new WaitForSeconds(1f);
        playerPool.StorePlayer();
        LevelBuilder.Instance.ClearScenary();
        LevelBuilder.Instance.LoadMenuLevel();
        playerPool.Player.TeleportTo(LevelBuilder.Instance.GetLevelMenuStartPoint());
        ShowTransition(false);
    }

    public void RestartLevel()
    {
        LevelBuilder.Instance.TooglelevelMenu(false);
        StartCoroutine(RestartLevelRoutine());
    }

    public void ContinueGame()
    {
        LevelBuilder.Instance.TooglelevelMenu(false);
        StartLevel(SaveManager.Instance.Data.currentLevelIndex);
        ScoreManager.Instance.LoadSavedScore();
    }
    
    public void NewGame()
    {
        ClearStateSave();
        StartLevel(0);
    }

    public void ClearStateSave()
    {
        ScoreManager.Instance.ResetScore();
        SaveManager.Instance.TotalScoreToSave(0);
        SaveManager.Instance.TotalDeathsToSave(0);
        SaveManager.Instance.Data.collectedCoins.Clear();
        SaveManager.Instance.SetCurrentLevelToSave(0);
    }

    public void LoadNextLevel()
    {
        // lógica de transição para próxima fase ou fim de jogo
        int nextIndex = levelBuilder.CurrentLevelIndex + 1;
        if (nextIndex <= levelBuilder.LastLevelIndex) // exemplo: 3 fases
        {
            StartLevel(nextIndex);
            SaveManager.Instance.SetCurrentLevelToSave(nextIndex);
        }
        else
        {
            GameEvents.GameOver(); // evento de fim de jogo
            // exibir tela final, placar, etc.
            Debug.Log("Acabou o jogo");
        }
    }

    public void EndLevel()
    {
        GameEvents.LevelComplete(); // evento de fim de fase
    }

    public void HomeButton()
    {
        StartCoroutine(ClearLevelRoutine());
    }

    private void ShowTransition(bool show)
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.alpha = show ? 1f : 0f;
            transitionCanvas.blocksRaycasts = show;
            transitionCanvas.interactable = show;
        }
    }
}