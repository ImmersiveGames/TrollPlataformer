using System.Collections;
using UnityEngine;

public class FallObstacle : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private float fallStartDelayTime;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float timeToDestroy = 5f;
    
    private bool hasFallen = false;
    
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
    
    private IEnumerator Fall()
    {
        if (hasFallen) yield break;
        hasFallen = true;
        
        yield return new WaitForSeconds(fallStartDelayTime);

        if (rb == null)
        {
            Debug.LogWarning("Rigidbody não atribuído!");
            yield break;
        }

        rb.isKinematic = false;

        // Aplica força extra de gravidade
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

        if (timeToDestroy > 0)
        {
            Destroy(gameObject, timeToDestroy);
        }
    }

    public void OnPlayerDetected()
    {
        //Debug.Log("cai cai");
        StartCoroutine(Fall());
    }
}
