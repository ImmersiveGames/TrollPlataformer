using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigationStarter : MonoBehaviour
{
    [SerializeField] private Selectable firstSelectable;

    private void OnEnable()
    {
        if (firstSelectable != null)
        {
            // Limpa seleção anterior para garantir que o novo seja selecionado
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }
    }
}
