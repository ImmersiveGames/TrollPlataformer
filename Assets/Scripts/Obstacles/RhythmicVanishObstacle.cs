using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmicVanishObstacle : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    [SerializeField] private List<Transform> obstacles;
    [SerializeField] private float timeBetweenSteps = 0.5f;
    [SerializeField] private float delayBeforeRestart = 1f;
    [SerializeField] private bool startVisible = true;
    [SerializeField] private bool reverseReappearOrder = false;

    private Coroutine cycleRoutine;
    private bool isCycleRunning = false;

    private void Awake()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected += OnPlayerDetected;

        // Define estado inicial
        foreach (Transform obstacle in obstacles)
        {
            obstacle.gameObject.SetActive(startVisible);
        }
    }

    private void OnDisable()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected -= OnPlayerDetected;
    }

    public void OnPlayerDetected()
    {
        if (!isCycleRunning)
        {
            cycleRoutine = StartCoroutine(VanishAndReappearCycle());
            isCycleRunning = true;
        }
    }

    private IEnumerator VanishAndReappearCycle()
    {
        while (true)
        {
            // Desaparecer em sequência
            foreach (Transform obstacle in obstacles)
            {
                obstacle.gameObject.SetActive(false);
                yield return new WaitForSeconds(timeBetweenSteps);
            }

            yield return new WaitForSeconds(delayBeforeRestart);

            // Reaparecer em sequência (normal ou invertida)
            List<Transform> reappearList = reverseReappearOrder ? new List<Transform>(obstacles) : obstacles;
            if (reverseReappearOrder)
                reappearList.Reverse();

            foreach (Transform obstacle in reappearList)
            {
                obstacle.gameObject.SetActive(true);
                yield return new WaitForSeconds(timeBetweenSteps);
            }

            yield return new WaitForSeconds(delayBeforeRestart);
        }
    }
}