using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPortal : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float teleportDelay = 0f;

    private bool hasTeleported = false;

    private void Awake()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected += OnPlayerDetected;
    }

    private void OnDisable()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected -= OnPlayerDetected;
    }

    public void OnPlayerDetected()
    {
        if (!hasTeleported && targetTransform != null)
        {
            StartCoroutine(TeleportPlayer());
            hasTeleported = true;
        }
    }

    private IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(teleportDelay);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerController>()?.TeleportTo(targetTransform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (targetTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(targetTransform.position, 0.3f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, targetTransform.position);
        }
    }
}