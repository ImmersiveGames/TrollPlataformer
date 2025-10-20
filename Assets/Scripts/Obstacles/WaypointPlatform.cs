using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointPlatform : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private Transform platform;
    [SerializeField] private float moveSpeed = 2f;
    
    [Header("Sensor")]
    [SerializeField] private PlayerDetectionSensor sensor;
    [SerializeField] private PlatformContactSensor contactSensor;
    
    [Header("Waypoints")]
    [SerializeField] public List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float delayBetweenPoints = 0f;
    [SerializeField] private bool pingPong = false;

    private int currentIndex = 0;
    private bool isActive = false;
    private bool forward = true;
    
    private Vector3 lastPosition;
    public Vector3 PlatformDelta { get; private set; } = Vector3.zero;
    private bool playerOnPlatform = false;
    private PlayerController playerController;

    void OnEnable()
    {
        if (sensor != null)
            sensor.OnPlayerDetected += ActivateSequence;
        
        if (contactSensor != null)
        {
            contactSensor.OnPlayerStepped += SetPlayerOnPlatform;
            contactSensor.OnPlayerLeft += ClearPlayer;
        }
    }

    void OnDisable()
    {
        if (sensor != null)
            sensor.OnPlayerDetected -= ActivateSequence;
        
        if (contactSensor != null)
        {
            contactSensor.OnPlayerStepped -= SetPlayerOnPlatform;
            contactSensor.OnPlayerLeft -= ClearPlayer;
        }
    }
    
    void Start()
    {
        lastPosition = platform.transform.position;
    }
    
    void Update()
    {
        PlatformDelta = platform.transform.position - lastPosition;
        lastPosition = platform.transform.position;

        if (playerOnPlatform && playerController != null)
        {
            playerController.ApplyPlatformDelta(PlatformDelta);
        }
    }

    private void ActivateSequence()
    {
        if (!isActive && waypoints.Count >= 1)
        {
            isActive = true;
            StartCoroutine(MoveSequence());
        }
    }

    private IEnumerator MoveSequence()
    {
        while (true)
        {
            Vector3 targetPos = waypoints[currentIndex].position;

            while (Vector3.Distance(platform.transform.position, targetPos) > 0.05f)
            {
                platform.transform.position = Vector3.MoveTowards(platform.transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (delayBetweenPoints > 0f)
                yield return new WaitForSeconds(delayBetweenPoints);

            if (pingPong)
            {
                if (forward)
                {
                    currentIndex++;
                    if (currentIndex >= waypoints.Count)
                    {
                        currentIndex = waypoints.Count - 2;
                        forward = false;
                    }
                }
                else
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = 1;
                        forward = true;
                    }
                }
            }
            else
            {
                currentIndex++;
                if (currentIndex >= waypoints.Count)
                    break;
            }
        }
    }
    
    public void SetPlayerOnPlatform(CharacterController controller)
    {
        playerController = controller.GetComponent<PlayerController>();
        playerOnPlatform = true;
        playerController.SetOnPlatform(true);
    }

    public void ClearPlayer()
    {
        playerController.SetOnPlatform(false);
        playerController = null;
        playerOnPlatform = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}