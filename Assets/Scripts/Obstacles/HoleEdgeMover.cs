using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleEdgeMover : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PlayerDetectionSensor playerDetectionSensor;
    [SerializeField] private Transform leftPlatform;
    [SerializeField] private Transform rightPlatform;

    [Header("Configuração de escala")]
    [SerializeField] private Vector2 leftScaleChange = new Vector2(0f, 0f);
    [SerializeField] private Vector2 rightScaleChange = new Vector2(0f, 0f);

    [Header("Configuração de movimento")]
    [SerializeField] private Vector2 leftPositionOffset = new Vector2(0f, 0f);
    [SerializeField] private Vector2 rightPositionOffset = new Vector2(0f, 0f);

    [Header("Tempo")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float transitionDuration = 1f;

    [Header("Opções")]
    [SerializeField] private bool returnToOriginal = false;
    [SerializeField] private float delayBeforeReturn = 1f;
    [SerializeField] private bool useCycle = false;
    [SerializeField] private float delayBetweenCycles = 2f;

    private Vector3 leftOriginalScale;
    private Vector3 rightOriginalScale;
    private Vector3 leftOriginalPosition;
    private Vector3 rightOriginalPosition;

    private bool isRunning = false;

    private void Awake()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected += OnPlayerDetected;

        if (leftPlatform != null)
        {
            leftOriginalScale = leftPlatform.localScale;
            leftOriginalPosition = leftPlatform.localPosition;
        }

        if (rightPlatform != null)
        {
            rightOriginalScale = rightPlatform.localScale;
            rightOriginalPosition = rightPlatform.localPosition;
        }
    }

    private void OnDisable()
    {
        if (playerDetectionSensor != null)
            playerDetectionSensor.OnPlayerDetected -= OnPlayerDetected;
    }

    public void OnPlayerDetected()
    {
        if (!isRunning)
        {
            if (useCycle)
                StartCoroutine(CycleRoutine());
            else
                StartCoroutine(AnimateOnceRoutine());
        }
    }

    private IEnumerator AnimateOnceRoutine()
    {
        isRunning = true;
        yield return new WaitForSeconds(startDelay);

        yield return AnimateToTarget();

        if (returnToOriginal)
        {
            yield return new WaitForSeconds(delayBeforeReturn);
            yield return AnimateToOriginal();
        }

        isRunning = false;
    }

    private IEnumerator CycleRoutine()
    {
        isRunning = true;
        yield return new WaitForSeconds(startDelay);

        while (useCycle)
        {
            yield return AnimateToTarget();
            yield return new WaitForSeconds(delayBeforeReturn);
            yield return AnimateToOriginal();
            yield return new WaitForSeconds(delayBetweenCycles);
        }

        isRunning = false;
    }

    private IEnumerator AnimateToTarget()
    {
        Vector3 leftTargetScale = leftOriginalScale + new Vector3(leftScaleChange.x, leftScaleChange.y, 0f);
        Vector3 rightTargetScale = rightOriginalScale + new Vector3(rightScaleChange.x, rightScaleChange.y, 0f);

        Vector3 leftTargetPosition = leftOriginalPosition + new Vector3(leftPositionOffset.x, leftPositionOffset.y, 0f);
        Vector3 rightTargetPosition = rightOriginalPosition + new Vector3(rightPositionOffset.x, rightPositionOffset.y, 0f);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;

            if (leftPlatform != null)
            {
                leftPlatform.localScale = Vector3.Lerp(leftOriginalScale, leftTargetScale, t);
                leftPlatform.localPosition = Vector3.Lerp(leftOriginalPosition, leftTargetPosition, t);
            }

            if (rightPlatform != null)
            {
                rightPlatform.localScale = Vector3.Lerp(rightOriginalScale, rightTargetScale, t);
                rightPlatform.localPosition = Vector3.Lerp(rightOriginalPosition, rightTargetPosition, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (leftPlatform != null)
        {
            leftPlatform.localScale = leftTargetScale;
            leftPlatform.localPosition = leftTargetPosition;
        }

        if (rightPlatform != null)
        {
            rightPlatform.localScale = rightTargetScale;
            rightPlatform.localPosition = rightTargetPosition;
        }
    }

    private IEnumerator AnimateToOriginal()
    {
        float elapsed = 0f;
        Vector3 leftStartScale = leftPlatform.localScale;
        Vector3 rightStartScale = rightPlatform.localScale;
        Vector3 leftStartPos = leftPlatform.localPosition;
        Vector3 rightStartPos = rightPlatform.localPosition;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;

            if (leftPlatform != null)
            {
                leftPlatform.localScale = Vector3.Lerp(leftStartScale, leftOriginalScale, t);
                leftPlatform.localPosition = Vector3.Lerp(leftStartPos, leftOriginalPosition, t);
            }

            if (rightPlatform != null)
            {
                rightPlatform.localScale = Vector3.Lerp(rightStartScale, rightOriginalScale, t);
                rightPlatform.localPosition = Vector3.Lerp(rightStartPos, rightOriginalPosition, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (leftPlatform != null)
        {
            leftPlatform.localScale = leftOriginalScale;
            leftPlatform.localPosition = leftOriginalPosition;
        }

        if (rightPlatform != null)
        {
            rightPlatform.localScale = rightOriginalScale;
            rightPlatform.localPosition = rightOriginalPosition;
        }
    }
}