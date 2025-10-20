using System;
using UnityEngine;

public class StartEndPoint : MonoBehaviour
{
    [SerializeField] Color color;
    [SerializeField] Vector3 gizmoSize;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Vector3 gizmoPosition = transform.position;
        gizmoPosition.y += gizmoSize.y / 2;
        Gizmos.DrawWireCube(gizmoPosition, gizmoSize);
    }
}
