using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed = 3f;
    private bool facingLeft = true;
    private bool canMoveForward = true;
    private bool isMoving = true;
    private Animator animator;
    // private GameObject player;
    [SerializeField] private List<Vector2> wayPoints = new List<Vector2>();
    private int currentWaypointIndex = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        // player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsCurrentWaypoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = false;
            UpdateWalkingAnimation(isMoving);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = true;
            UpdateWalkingAnimation(isMoving);
        }
    }

    private void MoveTowardsCurrentWaypoint()
    {
        if (wayPoints.Count == 0) // || currentWaypointIndex >= wayPoints.Count
        {
            return;
        }

        Vector2 targetPosition = wayPoints[currentWaypointIndex];

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        UpdateWalkingAnimation(true);

        if (Vector2.Distance((Vector2)transform.position, targetPosition) < 0.1f)
        {
            UpdateWaypointIndex();
        }

        bool shouldFlip = (transform.position.x > targetPosition.x && !facingLeft) || (transform.position.x < targetPosition.x && facingLeft);
        if (shouldFlip)
        {
            Flip();
        }
    }

    private void UpdateWaypointIndex()
    {
        currentWaypointIndex += canMoveForward ? +1 : -1;

        if (currentWaypointIndex >= wayPoints.Count)
        {
            currentWaypointIndex = wayPoints.Count - 1;
            canMoveForward = false;
        }
        else if (currentWaypointIndex < 0)
        {
            currentWaypointIndex = 0;
            canMoveForward = true;
        }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void UpdateWalkingAnimation(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }
}
