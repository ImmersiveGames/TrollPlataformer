using System;
using UnityEngine;

public class PlayerDetectionSensor : MonoBehaviour
{
    public event Action OnPlayerDetected;

    [Header("OverlapBox Settings")]
    [SerializeField] private Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 boxOffset = Vector3.zero;
    [SerializeField] private LayerMask detectionLayer;

    private bool isPlayerDetected = false;

    private void Update()
    {
        if (isPlayerDetected) return;

        Vector3 center = transform.position + boxOffset;
        Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, Quaternion.identity, detectionLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Transform parent = this.GetComponentInParent<Transform>();
                //Debug.Log(parent.name + " Detected: " + hit.name);
                OnPlayerDetected?.Invoke();
                isPlayerDetected = true;
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}