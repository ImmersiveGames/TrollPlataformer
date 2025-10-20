using UnityEngine;

public class PlayerSkinContainer : MonoBehaviour
{
    [SerializeField] private Transform skinContainer;

    public Transform GetSkinContainer() => skinContainer;
}