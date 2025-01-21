using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed;
    private bool facingLeft = true;
    private bool canMoveForward = true;
    private bool stopMove;

    private Animator animationNPC;
    private GameObject player;
    [SerializeField] private List<Vector2> positions = new List<Vector2>();
    private int currentIndex = 0;

    private void Start()
    {
        animationNPC = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (!stopMove)
        {
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stopMove = true;
            animationNPC.SetBool("isWalking", false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stopMove = false;
            animationNPC.SetBool("isWalking", true);
        }
    }

    private void Move()
    {
        if (positions.Count == 0 || currentIndex >= positions.Count)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, positions[currentIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, positions[currentIndex]) < 0.1f)
        {
            UpdatePositionIndex();
        }

        UpdateFacingDirection();
    }

    private void UpdatePositionIndex()
    {
        if (canMoveForward)
        {
            currentIndex++;
            if (currentIndex >= positions.Count)
            {
                currentIndex = positions.Count - 1;
                canMoveForward = false;
            }
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
                canMoveForward = true;
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if (transform.position.x > positions[currentIndex].x && !facingLeft)
        {
            Flip();
        }
        else if (transform.position.x < positions[currentIndex].x && facingLeft)
        {
            Flip();
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