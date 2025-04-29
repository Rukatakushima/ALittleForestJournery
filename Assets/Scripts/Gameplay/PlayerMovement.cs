using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameInput  _playerInput;
    
    private Rigidbody2D _rb;
    private Vector2 Velocity => _rb.velocity;

    [SerializeField] public float speed = 4;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask groundLayer;
    
    public bool IsFacingLeft { get; private set; }  = true;
    private bool IsGrounded => Physics2D.OverlapCircle(feetPos.position, checkRadius, groundLayer);
    
    [SerializeField] private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int TakeOf = Animator.StringToHash("takeOf");
    
    private void OnEnable()
    {
        _playerInput.Player.Jump.performed += OnJumpPerformed;
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Jump.performed -= OnJumpPerformed;
        _playerInput.Disable();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _playerInput = new();
        _playerInput.Enable();
    }

    private void FixedUpdate()
    {
        ReadMovementInput();
        UpdateAnimation();
    }
    
    private void ReadMovementInput()
    {
        var inputDirection = _playerInput.Player.Move.ReadValue<Vector2>();
        Move(inputDirection.x);
    }

    private void Move(float moveInput)
    {
        _rb.velocity = new Vector2(moveInput * speed, Velocity.y);

        // UpdateRunningAnimation();

        if ((IsFacingLeft && moveInput > 0) || (!IsFacingLeft && moveInput < 0))
            Flip();
    }

    // private void UpdateRunningAnimation() => animator.SetBool(IsWalking, Mathf.Abs(_rb.velocity.x) > 0);

    private void Flip()
    {
        IsFacingLeft = !IsFacingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        if (IsGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _rb.velocity = Vector2.up * jumpForce;
        animator.SetTrigger(TakeOf);
    }

    private void UpdateAnimation()
    {
        animator.SetBool(IsWalking, Mathf.Abs(_rb.velocity.x) > 0);
        animator.SetBool(IsJumping, !IsGrounded);
    }
}
