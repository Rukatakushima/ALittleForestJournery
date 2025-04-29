using UnityEngine;

public class Mover : MonoBehaviour
{
    private GameInput  _playerInput;
    
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    [SerializeField] public float speed = 4;
    private Rigidbody2D _rb;
    private Vector2 Velocity => _rb.velocity;
    [SerializeField] private Animator animator;
    public bool IsFacingLeft { get; private set; }  = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _playerInput = new();
        _playerInput.Enable();
    }

    private void FixedUpdate() => ReadMovementInput();//Move(Input.GetAxis("Horizontal"));

    private void ReadMovementInput()
    {
       var inputDirection = _playerInput.Player.Move.ReadValue<Vector2>();
       Move(inputDirection.x);
    }

    private void Move(float moveInput)
    {
        _rb.velocity = new Vector2(moveInput * speed, Velocity.y);

        UpdateRunningAnimation();

        if ((IsFacingLeft && moveInput > 0) || (!IsFacingLeft && moveInput < 0))
            Flip();
    }

    private void UpdateRunningAnimation() => animator.SetBool(IsWalking, Mathf.Abs(_rb.velocity.x) > 0);

    private void Flip()
    {
        IsFacingLeft = !IsFacingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
