using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 4;
    public float jumpForce, checkRadius;
    private Rigidbody2D rb;
    private bool facingLeft = true;
    private bool isGrounded;
    public Transform feetPos;
    public LayerMask whatIsGround;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if ((facingLeft && moveInput > 0) || (!facingLeft && moveInput < 0))
        {
            Flip();
        }

        anim.SetBool("isRunning", moveInput != 0);
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        
        if (isGrounded == true && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = Vector2.up * jumpForce;
            anim.SetTrigger("takeOf");
        }

        anim.SetBool("isJumping", !isGrounded);
    }

    void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}
