using UnityEngine;

public class EndLevelTrigger : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;

    private bool hasTriggered = false;

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
        if (!hasTriggered)
        {
            Debug.Log("EndLevelTrigger");
            hasTriggered = true;
            GameManager.Instance.EndLevel();
        }
    }
}