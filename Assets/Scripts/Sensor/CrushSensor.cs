using UnityEngine;
using System;
using System.Collections.Generic;

public class CrushSensor : MonoBehaviour
{
    [Header("Sensor Settings")]
    [SerializeField] private Vector3 sensorSize = new Vector3(0.8f, 1f, 0.8f);
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Detection Thresholds")]
    [SerializeField] private float verticalThreshold = 0.3f;
    [SerializeField] private float horizontalThreshold = 0.3f;
    
    bool alreadyCrushed = false;
    
    private void OnEnable()
    {
        GameEvents.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerRespawn -= OnPlayerRespawn;
    }
    
    private void Update()
    {
        if (alreadyCrushed) return;
        
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

    // Para esmagamento horizontal, vamos coletar as posições dos colisores à esquerda e à direita
    List<Collider> leftHits = new List<Collider>();
    List<Collider> rightHits = new List<Collider>();

    foreach (var hit in hits)
    {
        Vector3 direction = hit.transform.position - center;

        if (direction.y > verticalThreshold) hitAbove = true;
        if (direction.y < -verticalThreshold) hitBelow = true;

        if (direction.x > horizontalThreshold) rightHits.Add(hit);
        else if (direction.x < -horizontalThreshold) leftHits.Add(hit);
    }

    // Debug para esmagamento vertical
    //Debug.Log($"UP: {hitAbove}, Down: {hitBelow}");

    // Verifica esmagamento vertical
    bool verticalCrush = hitAbove && hitBelow;

    // Verifica esmagamento horizontal somente se houver colisores em ambos os lados
    bool horizontalCrush = false;
    if (leftHits.Count > 0 && rightHits.Count > 0)
    {
        // Encontra o colisor mais próximo à esquerda e o mais próximo à direita
        float maxLeftX = float.MinValue;
        foreach (var left in leftHits)
        {
            float leftX = left.transform.position.x;
            if (leftX > maxLeftX) maxLeftX = leftX;
        }

        float minRightX = float.MaxValue;
        foreach (var right in rightHits)
        {
            float rightX = right.transform.position.x;
            if (rightX < minRightX) minRightX = rightX;
        }

        // Calcula a distância horizontal entre os dois colisores mais próximos
        float distance = minRightX - maxLeftX;

        // Se a distância for menor que o tamanho do sensor na horizontal, considera esmagamento
        if (distance < sensorSize.x)
        {
            horizontalCrush = true;
        }
    }

    bool isCrushed = verticalCrush || horizontalCrush;
    alreadyCrushed = isCrushed;
    return isCrushed;
}


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, sensorSize);
    }

    public void OnPlayerRespawn()
    {
        alreadyCrushed = false;
    }
}

