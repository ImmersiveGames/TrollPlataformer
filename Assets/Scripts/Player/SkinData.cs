using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkinData", menuName = "TrollPlatformer/SkinData", order = 1)]
public class SkinData : ScriptableObject
{
    public string skinId;           // Identificador único da skin
    public string displayName;      // Nome para mostrar na loja
    public GameObject skinPrefab;   // Prefab da skin com modelo e Animator
    public int price;               // Preço em moedas
    public bool isDefault;          // Se é a skin padrão do jogador
    public Color skinIcon;          // Imagem da skin para a loja
}