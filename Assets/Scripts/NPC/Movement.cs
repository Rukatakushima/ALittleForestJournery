using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 3f;
    [SerializeField] private List<Vector2> wayPoints = new List<Vector2>();
    private bool _facingLeft = true;
    private bool _canMoveForward = true;
    private bool _isMoving = true;
    private int _currentWaypointIndex = 0;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void FixedUpdate()
    {
        if (_isMoving)
            MoveTowardsCurrentWaypoint();
    }

    public void SetWalking(bool isWalking)
    {
        _isMoving = isWalking;
        UpdateWalkingAnimation(_isMoving);
    }

    private void MoveTowardsCurrentWaypoint()
    {
        if (wayPoints.Count == 0) return;

        Vector2 targetPosition = wayPoints[_currentWaypointIndex];

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        UpdateWalkingAnimation(true);

        if (Vector2.Distance((Vector2)transform.position, targetPosition) < 0.1f)
            UpdateWaypointIndex();

        if ((transform.position.x > targetPosition.x && !_facingLeft) || (transform.position.x < targetPosition.x && _facingLeft))
            Flip();
    }

    private void UpdateWaypointIndex()
    {
        _currentWaypointIndex += _canMoveForward ? +1 : -1;

        if (_currentWaypointIndex >= wayPoints.Count)
        {
            _currentWaypointIndex = wayPoints.Count - 1;
            _canMoveForward = false;
        }
        else if (_currentWaypointIndex < 0)
        {
            _currentWaypointIndex = 0;
            _canMoveForward = true;
        }
    }

    private void Flip()
    {
        _facingLeft = !_facingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void UpdateWalkingAnimation(bool isWalking) => animator.SetBool(IsWalking, isWalking);
}
