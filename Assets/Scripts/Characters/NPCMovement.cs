using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed;
    private bool facingLeft = true; // смотрит влево
    bool canMoveForward;
    bool stopMove;
    Animator animationNPC;
    private GameObject player;

    [SerializeField]
    private List <Vector3> positions = new List<Vector3>();

    private int i = 0;
    private void Start()
    {
        animationNPC = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (!stopMove) Move();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        
        // if  (other.gameObject.CompareTag("Player"))
        // {
            stopMove = true;
            animationNPC.SetBool("isWalking", false);
        // }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        stopMove = false;
        animationNPC.SetBool("isWalking", true);
    }

    void Move()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, positions[i], speed*Time.deltaTime);
        animationNPC.SetBool("isWalking", true);

        if( transform.localPosition.x - positions[i].x > 0 && !facingLeft) 
        {
            Flip();
            facingLeft = true;
        }

        else if( transform.localPosition.x - positions[i].x < 0 && facingLeft) 
        {
            Flip();
            facingLeft = false;
        }

        if (transform.localPosition == positions[i]&&canMoveForward)
        {
            i++;

            if (i == positions.Count) //если прошли все позиции, то идем в обратном порядке
            {
                i--;
                canMoveForward = false;
            }
        }

        else if (transform.localPosition == positions[i]&&!canMoveForward) 
        {
            i--;
            if (i == -1) 
            {
                i = 0;
                canMoveForward = true;
            }
        }
    }

    void Flip() //переворот
    {
        facingLeft = !facingLeft;
        Vector3 Scaler = transform.localScale; //берем оригинальное положение игрока
        Scaler.x *= -1; //умножаем оригинальное положение игрока на -1 (переворачиваем)
        transform.localScale = Scaler; //применяем
    }

}
