using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private Transform skinContainer;
    [SerializeField] private List<SkinData> allSkins; // SkinData é ScriptableObject com prefab, nome, preço, etc

    private GameObject currentSkinInstance;
    private HashSet<string> unlockedSkins = new HashSet<string>();
    private string currentSkinId;

    private void Start()
    {
        skinContainer = GetComponent<PlayerPool>().Player.GetComponent<PlayerSkinContainer>().GetSkinContainer();
        LoadProgress();

        if (!string.IsNullOrEmpty(currentSkinId))
        {
            ApplySkin(currentSkinId);
        }
    }

    public void ApplySkin(string skinId)
    {
        // Verifica se a skin está desbloqueada
        if (IsSkinUnlocked(skinId))
        {
            // Busca o SkinData correspondente na lista
            SkinData skinToApply = allSkins.Find(skin => skin.skinId == skinId);
            if (skinToApply != null)
            {
                // Remove a skin atual, se existir
                if (currentSkinInstance != null)
                {
                    Destroy(currentSkinInstance);
                }
                // Instancia o prefab da nova skin no contêiner
                currentSkinInstance = Instantiate(skinToApply.skinPrefab, skinContainer);
                currentSkinId = skinId;
                SaveProgress(); // Salva o progresso para persistir a skin atual
            }
            else
            {
                Debug.LogWarning($"Skin com ID '{skinId}' não encontrada na lista allSkins.");
            }
        }
        else
        {
            Debug.LogWarning($"Skin com ID '{skinId}' não está desbloqueada.");
        }
    }

    public void UnlockSkin(string skinId)
    {
        // Adiciona skin ao unlockedSkins e salva progresso
        if (!unlockedSkins.Contains(skinId))
        {
            unlockedSkins.Add(skinId);
            SaveProgress(); // Método que salva o progresso (ex: PlayerPrefs, arquivo, etc)
        }
    }

    public bool IsSkinUnlocked(string skinId)
    {
        return unlockedSkins.Contains(skinId); // Retorna se skin está desbloqueada
    }

    public void SaveProgress()
    {
        // Salva currentSkinId e unlockedSkins
        SaveManager.Instance.Data.currentSkinID = currentSkinId;
        SaveManager.Instance.Data.unlockedSkinsIds = new List<string>(unlockedSkins);
    }

    public void LoadProgress()
    {
        currentSkinId = SaveManager.Instance.Data.currentSkinID;
        
        // Limpa o conjunto atual para evitar duplicatas
        unlockedSkins.Clear();

        // Verifica se a lista salva não é nula e adiciona os IDs ao HashSet
        if (SaveManager.Instance.Data.unlockedSkinsIds != null)
        {
            foreach (string skinId in SaveManager.Instance.Data.unlockedSkinsIds)
            {
                unlockedSkins.Add(skinId);
            }
        }
        
        // Garante que a skin padrão esteja desbloqueada
        SkinData defaultSkin = allSkins.Find(skin => skin.isDefault);
        if (defaultSkin != null && !unlockedSkins.Contains(defaultSkin.skinId))
        {
            unlockedSkins.Add(defaultSkin.skinId);
        }
    }
}