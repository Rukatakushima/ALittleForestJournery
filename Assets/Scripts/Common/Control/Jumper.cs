using UnityEngine;

public class Jumper : MonoBehaviour
{
    [SerializeField] public float jumpForce, checkRadius;
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        CheckGrounded();
        Jump();
        UpdateAnimation();
    }

    private void UpdateAnimation() => animator.SetBool("isJumping", !isGrounded);

    private void CheckGrounded() => isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, GroundLayer);

    private void Jump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            rb.velocity = Vector2.up * jumpForce;
            animator.SetTrigger("takeOf");
        }
    }
}