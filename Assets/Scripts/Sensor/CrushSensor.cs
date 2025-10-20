using UnityEngine;
using System;

public class CrushSensor : MonoBehaviour
{
    [Header("Sensor Settings")]
    [SerializeField] private Vector3 sensorSize = new Vector3(0.8f, 1f, 0.8f);
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Detection Thresholds")]
    [SerializeField] private float verticalThreshold = 0.3f;
    [SerializeField] private float horizontalThreshold = 0.3f;

    private void Update()
    {
        if (IsCrushed())
        {
            Debug.Log("Crushed");
            GameManager.Instance.OnPlayerDeath();
        }
    }

    private bool IsCrushed()
    {
        Vector3 center = transform.position;
        Collider[] hits = Physics.OverlapBox(center, sensorSize / 2f, Quaternion.identity, obstacleLayer);

        bool hitAbove = false;
        bool hitBelow = false;
        bool hitLeft = false;
        bool hitRight = false;

        foreach (var hit in hits)
        {
            Vector3 direction = hit.transform.position - center;

            if (direction.y > verticalThreshold) hitAbove = true;
            if (direction.y < -verticalThreshold) hitBelow = true;
            if (direction.x > horizontalThreshold) hitRight = true;
            if (direction.x < -horizontalThreshold) hitLeft = true;
        }

        // Esmagamento vertical ou horizontal
        return (hitAbove && hitBelow) || (hitLeft && hitRight);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, sensorSize);
    }
}