using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI walletText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameEvents.OnMoneyUpdated += UpdateWalletText;
        UpdateWalletText();
    }
    
    public void UpdateWalletText()
    {
        walletText.text = $"Money: {WalletManager.Instance.GetBalance()}";
    } 
}
