using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] public float speed = 4;
    [SerializeField] public float jumpForce;
    // checkRadius;
    // [SerializeField] private Transform feetPos;
    // [SerializeField] private LayerMask whatIsGround;

    private Rigidbody2D rb;
    private Animator anim;
    private bool facingLeft = true;

    // private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Move();
        // UpdateRunningAnimation();
    }

    public void Move(float moveInput)
    {
        // float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        UpdateRunningAnimation();

        if ((facingLeft && moveInput > 0) || (!facingLeft && moveInput < 0))
        {
            Flip();
        }
    }

    
    public void UpdateRunningAnimation()
    {
        anim.SetBool("isWalking", Mathf.Abs(rb.velocity.x) > 0);
    }

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    // private void UpdateAnimation()
    // {
    //     anim.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0);
    //     anim.SetBool("isJumping", !isGrounded);
    // }

    // private void CheckGrounded()
    // {
    //     isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
    // }

    private void Jump()
    {
        // if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        // {
        rb.velocity = Vector2.up * jumpForce;
        //     anim.SetTrigger("takeOf");
        // }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
