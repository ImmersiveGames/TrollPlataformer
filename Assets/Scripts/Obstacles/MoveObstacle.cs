using System.Collections;
using UnityEngine;

public class MoveObstacle: MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float moveSpeed;

    private bool hasMoved = false;
    
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
    
    private IEnumerator Move()
    {
        if(hasMoved) yield break;
        hasMoved = true;
        
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + moveDirection;

        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    public void OnPlayerDetected()
    {
        //Debug.Log("Move Move");
        StartCoroutine(Move());
    }
}
