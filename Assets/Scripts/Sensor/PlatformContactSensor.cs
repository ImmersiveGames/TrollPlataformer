using UnityEngine;
using System;

public class PlatformContactSensor : MonoBehaviour
{
    public event Action<CharacterController> OnPlayerStepped;
    public event Action OnPlayerLeft;

    [Header("OverlapBox Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1f, 0.5f, 1f);
    [SerializeField] private Vector3 boxOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private LayerMask detectionLayer;

    private bool isPlayerOnPlatform = false;

    private void Update()
    {
        Vector3 center = transform.position + boxOffset;
        Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, Quaternion.identity, detectionLayer);

        bool foundPlayer = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                if (!isPlayerOnPlatform)
                {
                    OnPlayerStepped?.Invoke(hit.GetComponent<CharacterController>());
                    isPlayerOnPlatform = true;
                }
                foundPlayer = true;
                break;
            }
        }

        if (!foundPlayer && isPlayerOnPlatform)
        {
            OnPlayerLeft?.Invoke();
            isPlayerOnPlatform = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}