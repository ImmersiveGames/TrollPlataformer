using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;

    [Header("Money Wallet")]
    [SerializeField] private int currentBalance = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadWallet();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount)
    {
        currentBalance += amount;
        SaveWallet();
        GameEvents.UpdateMoney(); // opcional: evento para UI
    }

    public bool SpendMoney(int amount)
    {
        if (CanAfford(amount))
        {
            currentBalance -= amount;
            SaveWallet();
            GameEvents.UpdateMoney(); // opcional: evento para UI
            return true;
        }
        return false;
    }

    public bool CanAfford(int amount)
    {
        return currentBalance >= amount;
    }

    public int GetBalance()
    {
        return currentBalance;
    }

    private void SaveWallet()
    {
        SaveManager.Instance.Data.walletBalance = currentBalance;
        SaveManager.Instance.SaveData();
    }

    private void LoadWallet()
    {
        currentBalance = SaveManager.Instance.GetWalletBalanceSaved();
        GameEvents.UpdateMoney();
    }

    public void ResetWallet()
    {
        currentBalance = 0;
        SaveWallet();
    }
}