using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] public float speed = 4;
    [SerializeField] public float jumpForce;
    public Vector2 Velocity => rb.velocity;

    // [SerializeField] private Transform feetPos;
    // [SerializeField] private LayerMask whatIsGround;

    private Rigidbody2D rb;
    private Animator anim;
    public bool isFacingLeft = true;

    // private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move(Input.GetAxis("Horizontal"));
        // UpdateRunningAnimation();
    }

    public void Move(float moveInput)
    {
        // float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        UpdateRunningAnimation();

        if ((isFacingLeft && moveInput > 0) || (!isFacingLeft && moveInput < 0))
            Flip();
    }


    public void UpdateRunningAnimation() => anim.SetBool("isWalking", Mathf.Abs(rb.velocity.x) > 0);


    // private void UpdateAnimation()
    // {
    //     anim.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0);
    //     anim.SetBool("isJumping", !isGrounded);
    // }

    // private void CheckGrounded()
    // {
    //     isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    // }

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
