using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] public float speed = 4;
    public Vector2 Velocity => rb.velocity;
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    public bool isFacingLeft = true;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate() => Move(Input.GetAxis("Horizontal"));

    public void Move(float moveInput)
    {
        rb.velocity = new Vector2(moveInput * speed, Velocity.y);

        UpdateRunningAnimation();

        if ((isFacingLeft && moveInput > 0) || (!isFacingLeft && moveInput < 0))
            Flip();
    }


    public void UpdateRunningAnimation() => animator.SetBool("isWalking", Mathf.Abs(rb.velocity.x) > 0);

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
