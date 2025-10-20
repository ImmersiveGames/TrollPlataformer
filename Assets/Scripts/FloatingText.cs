using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Configuração Visual")]
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private CanvasGroup canvasGroup;

    private float timer = 0f;

    public void Setup(string message, Color color)
    {
        textMesh.text = message;
        textMesh.color = color;
    }

    void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(gameObject);
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
        }
    }
}