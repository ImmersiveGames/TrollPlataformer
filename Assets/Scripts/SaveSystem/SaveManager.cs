using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public SaveData Data { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        Data = SaveSystem.Load();
    }

    public void SaveData()
    {
        SaveSystem.Save(Data);
    }

    // Atualiza o índice da fase atual
    public void SetCurrentLevelToSave(int levelIndex)
    {
        Data.currentLevelIndex = levelIndex;
        SaveData();
    }

    // Adiciona à pontuação total
    public void TotalScoreToSave(int amount)
    {
        Data.totalScore = amount;
        SaveData();
    }

    // Adiciona à contagem total de mortes
    public void TotalDeathsToSave(int totalDeaths)
    {
        Data.totalDeaths = totalDeaths;
        SaveData();
    }

    // Resetar progresso (opcional)
    public void ResetProgress()
    {
        Data = new SaveData();
        SaveData();
    }

    public int GetWalletBalanceSaved()
    {
       return Data.walletBalance;
    }

    public void SaveSkinsData(string currentSkinId, List<string> unlockedSkins)
    {
        Data.currentSkinID = currentSkinId;
        Data.unlockedSkinsIds = new List<string>(unlockedSkins);
        SaveData();
    }
}