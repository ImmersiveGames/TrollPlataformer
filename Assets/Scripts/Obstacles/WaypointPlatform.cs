using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform platform;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arriveThreshold = 0.05f;

    [Header("Waypoints")]
    [SerializeField] public List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float delayBetweenPoints = 0f;
    [SerializeField] private bool pingPong = false;

    [Header("Sensors (opcionais)")]
    [SerializeField] private PlayerDetectionSensor sensor;
    [SerializeField] private PlatformContactSensor contactSensor;

    [Header("Debug")]
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool debugLogs = false;

    private int currentIndex = 0;
    private bool isActive = false;
    private int dir = 1; // +1 frente, -1 trás

    private Vector3 lastPosition;
    public Vector3 PlatformDelta { get; private set; } = Vector3.zero;

    private bool playerOnPlatform = false;
    private PlayerController playerController;

    private void OnEnable()
    {
        if (sensor != null) sensor.OnPlayerDetected += ActivateSequence;
        if (contactSensor != null)
        {
            contactSensor.OnPlayerStepped += SetPlayerOnPlatform;
            contactSensor.OnPlayerLeft += ClearPlayer;
        }
    }

    private void OnDisable()
    {
        if (sensor != null) sensor.OnPlayerDetected -= ActivateSequence;
        if (contactSensor != null)
        {
            contactSensor.OnPlayerStepped -= SetPlayerOnPlatform;
            contactSensor.OnPlayerLeft -= ClearPlayer;
        }
    }

    private void Start()
    {
        if (platform == null) platform = transform;
        lastPosition = platform.position;

        // Aviso automático: waypoint como filho da plataforma (evita bug)
        if (waypoints != null)
        {
            foreach (var wp in waypoints)
            {
                if (wp != null && wp.IsChildOf(platform))
                    Debug.LogWarning($"Waypoint '{wp.name}' é filho da plataforma '{platform.name}'. Mova-o para fora da hierarquia da plataforma.");
            }
        }

        if (autoStart)
            ActivateSequence();
    }

    private void Update()
    {
        PlatformDelta = platform.position - lastPosition;
        lastPosition = platform.position;

        if (playerOnPlatform && playerController != null)
            playerController.ApplyPlatformDelta(PlatformDelta);
    }

    public void ActivateSequence()
    {
        if (isActive) return;
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning("WaypointPlatform: nenhum waypoint configurado.");
            return;
        }

        // Start mesmo com 1 waypoint — ele deve ir até o ponto e ficar lá
        isActive = true;
        currentIndex = Mathf.Clamp(currentIndex, 0, waypoints.Count - 1);
        dir = 1;
        StartCoroutine(MoveSequence());
    }

    private IEnumerator MoveSequence()
    {
        float arriveSqr = arriveThreshold * arriveThreshold;

        // Caso 1: apenas um waypoint — mover até ele e ficar
        if (waypoints.Count == 1)
        {
            Vector3 target = waypoints[0].position;
            while ((platform.position - target).sqrMagnitude > arriveSqr)
            {
                platform.position = Vector3.MoveTowards(platform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (debugLogs) Debug.Log("[WaypointPlatform] Chegou no único waypoint e parou.");
            isActive = false;
            yield break;
        }

        // Caso N (>=2): ping-pong ou linear
        while (true)
        {
            Vector3 targetPos = waypoints[currentIndex].position;

            while ((platform.position - targetPos).sqrMagnitude > arriveSqr)
            {
                platform.position = Vector3.MoveTowards(platform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (debugLogs) Debug.Log($"[WaypointPlatform] Chegou em {currentIndex}");

            if (delayBetweenPoints > 0f)
                yield return new WaitForSeconds(delayBetweenPoints);

            if (pingPong)
            {
                // Inverte direção imediatamente ao estar em extremidade
                if (currentIndex == waypoints.Count - 1) dir = -1;
                else if (currentIndex == 0) dir = 1;

                currentIndex += dir;

                // segurança: clamp
                currentIndex = Mathf.Clamp(currentIndex, 0, waypoints.Count - 1);
            }
            else
            {
                currentIndex++;
                if (currentIndex >= waypoints.Count)
                {
                    if (debugLogs) Debug.Log("[WaypointPlatform] Sequência linear finalizada.");
                    isActive = false;
                    yield break;
                }
            }

            yield return null;
        }
    }

    // Player handling
    public void SetPlayerOnPlatform(CharacterController controller)
    {
        playerController = controller.GetComponent<PlayerController>();
        playerOnPlatform = true;
        if (playerController != null) playerController.SetOnPlatform(true);
    }

    public void ClearPlayer()
    {
        if (playerController != null) playerController.SetOnPlatform(false);
        playerController = null;
        playerOnPlatform = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (waypoints == null) return;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
