using UnityEngine;
using UnityEngine.InputSystem;

public class Jumper : MonoBehaviour
{
    private GameInput _playerInput;
    
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    
    private bool IsGrounded => Physics2D.OverlapCircle(feetPos.position, checkRadius, groundLayer);
    private Rigidbody2D _rb;
    
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int TakeOf = Animator.StringToHash("takeOf");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = new GameInput();
    }

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

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (IsGrounded)
        {
            _rb.velocity = Vector2.up * jumpForce;
            animator.SetTrigger(TakeOf);
        }
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation() => animator.SetBool(IsJumping, !IsGrounded);
}