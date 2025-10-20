using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null) 
            animator = GetComponent<Animator>();

        GameEvents.OnLevelComplete += LevelComplete;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelComplete -= LevelComplete;
    }

    public void MovementAnimation(float movement)
    {
        animator.SetFloat("Movement", movement);
    }

    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }

    public void PlayDeathNormal()
    {
        animator.SetTrigger("Death_Normal");
    }

    public void PlayDeathCrushVertical()
    {
        animator.SetTrigger("Death_Crush_Vertical");
    }

    public void PlayDeathCrushHorizontal()
    {
        animator.SetTrigger("Death_Crush_Horizontal");
    }

    public void Respawn()
    {
        animator.CrossFade("idle", 0.1f);
    }

    public void LevelComplete()
    {
        animator.SetFloat("Movement", 0f);
    }

}
