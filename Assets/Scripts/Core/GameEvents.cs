using System;

public static class GameEvents
{
    public static event Action OnStartGame;
    public static event Action OnRestartGame;
    public static event Action OnPlayerDie;
    public static event Action OnPlayerRespawn;
    public static event Action OnLevelComplete;
    public static event Action OnGameOver;
    public static event Action OnScoreUpdated;
    public static event Action OnMoneyUpdated;
    public static event Action OnCleanupTrolls;

    public static void StartGame() => OnStartGame?.Invoke();
    public static void RestartGame() => OnRestartGame?.Invoke();
    public static void PlayerDie() => OnPlayerDie?.Invoke();
    public static void PlayerRespawn() => OnPlayerRespawn?.Invoke();
    public static void LevelComplete() => OnLevelComplete?.Invoke();
    public static void GameOver() => OnGameOver?.Invoke();
    public static void UpdateScore() => OnScoreUpdated?.Invoke();
    public static void UpdateMoney() => OnMoneyUpdated?.Invoke();
    public static void CleanTrolls() => OnCleanupTrolls?.Invoke();
}