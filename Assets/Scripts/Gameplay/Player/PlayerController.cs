using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float jumpForce, checkRadius;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask whatIsGround;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckGrounded();
        Jump();
        UpdateAnimation();
    }

    private void UpdateAnimation() => anim.SetBool("isJumping", !isGrounded);
    // anim.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0);

    private void CheckGrounded() => isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

    private void Jump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = Vector2.up * jumpForce;
            anim.SetTrigger("takeOf");
        }
    }
}