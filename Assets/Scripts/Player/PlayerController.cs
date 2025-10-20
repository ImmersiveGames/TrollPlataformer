using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputActions inputActions;

    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float maxJumpTime = 0.5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float ceilingCheckRadius = 0.2f;
    [SerializeField] private LayerMask ceilingLayer;

    private PlayerAnimation playerAnimation;
    private Vector2 moveInput;
    private float verticalVelocity;
    private bool isJumping;
    private float jumpTimeCounter;
    
    private Vector3 externalDelta = Vector3.zero;
    
    private bool isOnPlatform = false;

    private void Awake()
    {
        characterController ??= GetComponent<CharacterController>();
        playerAnimation ??= GetComponent<PlayerAnimation>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.started += ctx => StartJump();
        inputActions.Player.Jump.canceled += ctx => StopJump();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        if (!characterController.enabled) return;
        HandleMovement();
        HandleFlip();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x * moveSpeed, verticalVelocity, 0);
        //Debug.Log("Move: " + move);
        
        // Aplique delta vertical da plataforma
        move.y += externalDelta.y / Time.deltaTime;

        // Aplique delta horizontal da plataforma
        move.x += externalDelta.x / Time.deltaTime;
        
        if (isOnPlatform)
        {
            verticalVelocity = -500f;
        }
        else if (IsGrounded() || isOnPlatform)
        {
            if (!isJumping)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (isJumping && jumpTimeCounter < maxJumpTime)
        {
            jumpTimeCounter += Time.deltaTime;
            verticalVelocity = Mathf.Sqrt(2 * -gravity * jumpHeight);
            
            if (HitCeiling())
            {
                //Debug.Log("Ceiling hit");
                isJumping = false;
                verticalVelocity = -2f; // força início da queda
            }
        }

        move.y = verticalVelocity;
        characterController.Move(move * Time.deltaTime);
        externalDelta = Vector3.zero; // zera após aplicar

        float horizontalSpeed = Mathf.Abs(moveInput.x);
        playerAnimation.MovementAnimation(horizontalSpeed);
    }

    private bool IsGrounded()
    {
        float rayLength = characterController.skinWidth + 0.2f;
        bool rayHit = Physics.Raycast(transform.position, Vector3.down, rayLength);
        return characterController.isGrounded || rayHit || isOnPlatform;
    }

    private void StartJump()
    {
        if (IsGrounded())
        {
            isOnPlatform = false;
            isJumping = true;
            verticalVelocity = Mathf.Sqrt(2 * -gravity * jumpHeight);
            jumpTimeCounter = 0f;
            playerAnimation.PlayJump();
        }
    }

    private void StopJump() => isJumping = false;

    private void HandleFlip()
    {
        if (moveInput.x > 0)
            Flip(false);
        else if (moveInput.x < 0)
            Flip(true);
    }

    public void Flip(bool flip)
    {
        transform.localScale = flip ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        //Debug.Log("Flip chamado: " + flip);
    }

    public void ToogleCharacterController(bool state)
    {
        if (characterController != null)
            characterController.enabled = state;
        else
            Debug.LogError("CharacterController is null");
    }

    public void TeleportTo(Vector3 position)
    {
        ToogleCharacterController(false);
        transform.position = position;
        verticalVelocity = 0f;
        moveInput = Vector2.zero;
        ToogleCharacterController(true);
    }

    public void ApplyPlatformDelta(Vector3 delta)
    {
        externalDelta += delta;
    }
    
    public void SetOnPlatform(bool state)
    {
        isOnPlatform = state;
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("MovingPlatform"))
        {
            SetOnPlatform(true);
        }
    }
    
    private bool HitCeiling()
    {
        return Physics.CheckSphere(ceilingCheck.position, ceilingCheckRadius, ceilingLayer);
    }
}