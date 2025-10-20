using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObstacle : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    [SerializeField] private Transform targetObstacle;
    [SerializeField] private bool scaleX = true;
    [SerializeField] private bool scaleY = false;
    [SerializeField] private float scaleAmountX = 1f;
    [SerializeField] private float scaleAmountY = 1f;
    [SerializeField] private float scaleDuration = 0.5f;

    [Header("Retorno ao tamanho original")]
    [SerializeField] private bool returnToOriginal = false;
    [SerializeField] private float delayBeforeReturn = 1f;

    [Header("Ciclo de crescimento e redução")]
    [SerializeField] private bool useCycle = false;
    [SerializeField] private float delayBetweenCycles = 2f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isScaling = false;

    private void Awake()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected += OnPlayerDetected;

        if (targetObstacle != null)
        {
            originalScale = targetObstacle.localScale;
            originalPosition = targetObstacle.localPosition;
        }
    }

    private void OnDisable()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected -= OnPlayerDetected;
    }

    public void OnPlayerDetected()
    {
        if (!isScaling)
        {
            if (useCycle)
                StartCoroutine(AlternateScaleRoutine());
            else
                StartCoroutine(ScaleOnceRoutine());
        }
    }

    private IEnumerator ScaleOnceRoutine()
    {
        isScaling = true;

        Vector3 targetScale = GetTargetScale();
        Vector3 targetPosition = GetAdjustedPosition(targetScale);
        yield return ScaleTo(targetScale, targetPosition);

        if (returnToOriginal)
        {
            yield return new WaitForSeconds(delayBeforeReturn);
            yield return ScaleTo(originalScale, originalPosition);
        }

        isScaling = false;
    }

    private IEnumerator AlternateScaleRoutine()
    {
        isScaling = true;

        while (useCycle)
        {
            Vector3 targetScale = GetTargetScale();
            Vector3 targetPosition = GetAdjustedPosition(targetScale);
            yield return ScaleTo(targetScale, targetPosition);

            yield return new WaitForSeconds(delayBeforeReturn);
            yield return ScaleTo(originalScale, originalPosition);

            yield return new WaitForSeconds(delayBetweenCycles);
        }

        isScaling = false;
    }

    private Vector3 GetTargetScale()
    {
        Vector3 target = originalScale;

        if (scaleX)
            target.x += scaleAmountX;

        if (scaleY)
            target.y += scaleAmountY;

        return target;
    }

    private Vector3 GetAdjustedPosition(Vector3 targetScale)
    {
        Vector3 offset = Vector3.zero;

        if (scaleY)
            offset.y = (targetScale.y - originalScale.y) / 2f;

        if (scaleX)
            offset.x = (targetScale.x - originalScale.x) / 2f;

        return originalPosition + offset;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, Vector3 targetPosition)
    {
        float elapsed = 0f;
        Vector3 startScale = targetObstacle.localScale;
        Vector3 startPosition = targetObstacle.localPosition;

        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            targetObstacle.localScale = Vector3.Lerp(startScale, targetScale, t);
            targetObstacle.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetObstacle.localScale = targetScale;
        targetObstacle.localPosition = targetPosition;
    }
}