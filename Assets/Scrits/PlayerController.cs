using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 4;
    public float jumpForce;
    public float moveInput;

    public Joystick joystick;

    private Rigidbody2D rb;

    private bool facingLeft = true; //игрок смотрит влево

    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        //moveInput = joystick.Horizontal;
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        if(facingLeft == false && moveInput < 0) //если игрок смотрит не влево (вправо) и клавиша нажата
        {
            Flip();
        }
        else if(facingLeft == true && moveInput > 0)
        {
            Flip();
        }
        if(moveInput == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);

        }

    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
//if(isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        if(isGrounded == true && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))) //если игрок на земле и нажата клавиша "пробел"
        {
            rb.velocity = Vector2.up * jumpForce; //то прыгаем
            anim.SetTrigger("takeOf");
        }

        if(isGrounded == true) //если мы на земле, то не прыгаем, в ином случае - прыгаем
        {
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
        }
    }

    void Flip() //переворот
    {
        facingLeft = !facingLeft; //поворачивается
        Vector3 Scaler = transform.localScale; //берем оригинальное положение игрока
        Scaler.x *= -1; //умножаем оригинальное положение игрока на -1 (переворачиваем)
        transform.localScale = Scaler; //применяем
/*
        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 100, 0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }*/
    }
}
