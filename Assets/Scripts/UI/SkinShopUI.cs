using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinShopUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // Content da Scroll View com Grid Layout Group
    [SerializeField] private GameObject skinItemPrefab; // Prefab do item da skin (imagem + botão)

    private SkinManager skinManager;
    private WalletManager walletManager;

    private void Start()
    {
        skinManager = SkinManager.Instance;
        walletManager = WalletManager.Instance;
    }

    private void PopulateSkinItems()
    {
        // Limpa itens antigos
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (SkinData skin in skinManager.GetAllSkinsList())
        {
            GameObject itemGO = Instantiate(skinItemPrefab, contentParent);

            // Configura a imagem (por enquanto só muda a cor para diferenciar)
            Image img = itemGO.GetComponentInChildren<Image>();
            if (img != null)
            {
                img.color = skin.isDefault ? Color.gray : Color.white; // Exemplo: skin padrão cinza, outras brancas
            }

            // Configura o botão
            Button btn = itemGO.GetComponentInChildren<Button>();
            if (btn != null)
            {
                TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                UpdateButton(btn, btnText, skin);

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSkinButtonClicked(skin));
            }
        }
    }

    private void UpdateButton(Button btn, TextMeshProUGUI btnText, SkinData skin)
    {
        if (skinManager.IsSkinUnlocked(skin.skinId))
        {
            btnText.text = skinManager.GetCurrentSkinId() == skin.skinId ? "Equipado" : "Equipar";
            btn.interactable = skinManager.GetCurrentSkinId() != skin.skinId;
        }
        else
        {
            btnText.text = $"Comprar ({skin.price} moedas)";
            btn.interactable = walletManager.CanAfford(skin.price);
        }
    }

    private void OnSkinButtonClicked(SkinData skin)
    {
        if (skinManager.IsSkinUnlocked(skin.skinId))
        {
            skinManager.ApplySkin(skin.skinId);
            PopulateSkinItems(); // Atualiza botões
        }
        else
        {
            if (walletManager.SpendMoney(skin.price))
            {
                skinManager.UnlockSkin(skin.skinId);
                skinManager.ApplySkin(skin.skinId);
                PopulateSkinItems(); // Atualiza botões
            }
            else
            {
                Debug.Log("Saldo insuficiente para comprar a skin.");
                // Aqui você pode disparar um aviso na UI para o jogador
            }
        }
    }

    public void OnEnterSkinStore()
    {
        PopulateSkinItems();
    }
}