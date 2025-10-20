using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishObstacle : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    [SerializeField] private List<Transform> obstaclesToVanish;
    [SerializeField] private float timeToVanish = 1f;
    [SerializeField] private bool useSequentialVanish = false;
    [SerializeField] private bool unvanishPlataform = false;

    private bool hasVanished = false;

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

    private IEnumerator VanishObstacles()
    {
        if (hasVanished) yield break;

        yield return new WaitForSeconds(timeToVanish);

        if (useSequentialVanish)
        {
            foreach (Transform obstacle in obstaclesToVanish)
            {
                obstacle.gameObject.SetActive(unvanishPlataform);
                yield return new WaitForSeconds(timeToVanish); // espera entre cada plataforma
            }
        }
        else
        {
            foreach (Transform obstacle in obstaclesToVanish)
            {
                obstacle.gameObject.SetActive(unvanishPlataform);
            }
        }

        hasVanished = true;
    }

    public void OnPlayerDetected()
    {
        StartCoroutine(VanishObstacles());
    }
}