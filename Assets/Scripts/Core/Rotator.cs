using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Velocidade de Rotação")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f); // graus por segundo

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}