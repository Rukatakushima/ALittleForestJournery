using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed = 4;
    [SerializeField] public float jumpForce, checkRadius;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask whatIsGround;

    private Rigidbody2D rb;
    private Animator anim;
    private bool facingLeft = true;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
        UpdateAnimation();
    }

    private void Update()
    {
        CheckGrounded();
        Jump();
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if ((facingLeft && moveInput > 0) || (!facingLeft && moveInput < 0))
        {
            Flip();
        }
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0);
        anim.SetBool("isJumping", !isGrounded);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    }

    private void Jump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = Vector2.up * jumpForce;
            anim.SetTrigger("takeOf");
        }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
