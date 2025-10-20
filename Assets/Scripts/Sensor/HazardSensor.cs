using System;
using UnityEngine;

public class HazardSensor : MonoBehaviour
{
    [Header("Capsule Settings")]
    [SerializeField] private float capsuleHeight = 2f;
    [SerializeField] private float capsuleRadius = 0.5f;
    [SerializeField] private Vector3 capsuleOffset = Vector3.up * 0.5f;
    [SerializeField] private LayerMask hazardLayer;

    private bool hazardDetected = false;

    private void Start()
    {
        GameEvents.OnPlayerRespawn += ResetSensor;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerRespawn -= ResetSensor;
    }

    private void Update()
    {
        if (hazardDetected) return;
        

        Vector3 center = transform.position + capsuleOffset;
        Vector3 point0 = center + Vector3.up * (capsuleHeight / 2f - capsuleRadius);
        Vector3 point1 = center + Vector3.down * (capsuleHeight / 2f - capsuleRadius);

        Collider[] hits = Physics.OverlapCapsule(point0, point1, capsuleRadius, hazardLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Hazard"))
            {
                Debug.Log("HAZARD DETECTED: " + hit.name);
                GameManager.Instance.OnPlayerDeath();
                hazardDetected = true;
                break;
            }
        }
    }

	public void ResetSensor() { hazardDetected = false; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Vector3 center = transform.position + capsuleOffset;
        Vector3 point0 = center + Vector3.up * (capsuleHeight / 2f - capsuleRadius);
        Vector3 point1 = center + Vector3.down * (capsuleHeight / 2f - capsuleRadius);

        Gizmos.DrawWireSphere(point0, capsuleRadius);
        Gizmos.DrawWireSphere(point1, capsuleRadius);
        Gizmos.DrawLine(point0 + Vector3.forward * capsuleRadius, point1 + Vector3.forward * capsuleRadius);
        Gizmos.DrawLine(point0 - Vector3.forward * capsuleRadius, point1 - Vector3.forward * capsuleRadius);
        Gizmos.DrawLine(point0 + Vector3.right * capsuleRadius, point1 + Vector3.right * capsuleRadius);
        Gizmos.DrawLine(point0 - Vector3.right * capsuleRadius, point1 - Vector3.right * capsuleRadius);
    }
}