using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed;
    private bool FacingLeft = true;
    private bool CanMoveForward = true;
    private bool isMoving = true;
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
        if (isMoving)
        {
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = false;
            SetWalkingAnimation(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = true;
            SetWalkingAnimation(true);
        }
    }

    private void Move()
    {
        if (positions.Count == 0 || currentIndex >= positions.Count)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, positions[currentIndex], speed * Time.deltaTime);
        SetWalkingAnimation(true);

        if (Vector2.Distance((Vector2)transform.position, positions[currentIndex]) < 0.1f)
        {
            UpdatePositionIndex();
        }

        UpdateFacingDirection();
    }

    private void UpdatePositionIndex()
    {
        if (CanMoveForward)
        {
            currentIndex++;
            if (currentIndex >= positions.Count)
            {
                currentIndex = positions.Count - 1;
                CanMoveForward = false;
            }
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
                CanMoveForward = true;
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if ((transform.position.x > positions[currentIndex].x && !FacingLeft) || (transform.position.x < positions[currentIndex].x && FacingLeft))
        {
            Flip();
        }
    }

    private void Flip()
    {
        FacingLeft = !FacingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void SetWalkingAnimation(bool isWalking)
    {
        animationNPC.SetBool("isWalking", isWalking);
    }
}
