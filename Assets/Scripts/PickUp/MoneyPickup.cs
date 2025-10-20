using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [Header("Configuração da Moeda")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private bool generateIDAutomatically = true;
    [SerializeField] private string coinID;
    [SerializeField] private GameObject floatingTextPrefab;

    [Header("Trollagem")]
    [SerializeField] private GameObject trollCoinPrefab;

    private bool collected = false;

    private void GenerateID()
    {
        if (generateIDAutomatically)
        {
            string levelName = LevelBuilder.Instance.GetLevelName();
            Vector3 pos = transform.position;
            coinID = $"{levelName}_coin_{Mathf.RoundToInt(pos.x)}_{Mathf.RoundToInt(pos.y)}_{Mathf.RoundToInt(pos.z)}";
        }
    }

    void Start()
    {
        GenerateID();
        
        if (SaveManager.Instance.Data.collectedCoins.Contains(coinID))
        {
            ActivateTrollMode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || other.CompareTag("Player") == false) return;

        collected = true;
        WalletManager.Instance.AddMoney(coinValue);
        SaveManager.Instance.Data.collectedCoins.Add(coinID);
        SaveManager.Instance.SaveData();
        ShowMoneyCollectedText();
        Destroy(gameObject);
    }
    
    void ShowMoneyCollectedText()
    {
        if (floatingTextPrefab == null) return;

        string phrase = $"+ {coinValue}";
        Vector3 spawnPos = transform.position + Vector3.up;

        GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        if (floatingText != null)
            floatingText.Setup(phrase, Color.green);
    }

    private void ActivateTrollMode()
    {
        if (trollCoinPrefab != null)
        {
            Instantiate(trollCoinPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}