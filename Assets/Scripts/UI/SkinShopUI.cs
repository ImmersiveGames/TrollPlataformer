using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkinShopUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // Content da Scroll View com Grid Layout Group
    [SerializeField] private GameObject skinItemPrefab; // Prefab do item da skin (imagem + botão)

    private SkinManager skinManager;
    private WalletManager walletManager;
    private Button firstButton;

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

        firstButton = null;

        foreach (SkinData skin in skinManager.GetAllSkinsList())
        {
            GameObject itemGO = Instantiate(skinItemPrefab, contentParent);

            // Configura a imagem (por enquanto só muda a cor para diferenciar)
            Image img = itemGO.GetComponentInChildren<SkinItemShopUI>().skinImage;
            if (img != null)
            {
                img.color = skin.skinIcon; // Exemplo: skin padrão cinza, outras brancas
            }

            // Configura o botão
            Button btn = itemGO.GetComponentInChildren<SkinItemShopUI>().skinButton;
            if (btn != null)
            {
                TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                UpdateButton(btn, btnText, skin);

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnSkinButtonClicked(skin));

                // Prioriza o botão da skin equipada como firstButton
                if (firstButton == null || skinManager.GetCurrentSkinId() == skin.skinId)
                {
                    firstButton = btn;
                }
            }
        }

        // Define o botão selecionado no EventSystem
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null); // Limpa seleção anterior
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
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
        }
        else
        {
            if (walletManager.SpendMoney(skin.price))
            {
                skinManager.UnlockSkin(skin.skinId);
                skinManager.ApplySkin(skin.skinId);
            }
            else
            {
                Debug.Log("Saldo insuficiente para comprar a skin.");
                // Aqui você pode disparar um aviso na UI para o jogador
                return;
            }
        }

        PopulateSkinItems();

        // Após atualizar os botões, refoca o botão da skin equipada para manter navegação
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    public void OnEnterSkinStore()
    {
        PopulateSkinItems();
    }
}