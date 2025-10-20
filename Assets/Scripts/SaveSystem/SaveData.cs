using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int currentLevelIndex;
    public int totalScore;
    public int totalDeaths;
    public int walletBalance;
    public List<string> collectedCoins;

    public string currentSkinID;
    public List<string> unlockedSkinsIds;

    public int saveVersion = 1;
}