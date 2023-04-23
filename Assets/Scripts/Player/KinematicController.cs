using UnityEngine;

public class KinematicController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float totalJumpTime = 0.5f;
    [SerializeField] private float timeToJumpApex = 0.2f;
    [SerializeField] private float distanceToGroundThreshold = 0.05f;

    private float jumpVelocity;
    private Vector2 velocity;
    private float gravity;
    private bool isGrounded;

    private void Start()
    {
        CalculateJumpVelocity();
        gravity = -Physics2D.gravity.y;
        velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Calculate movement acceleration
        Vector2 acceleration = new Vector2(horizontalInput * moveSpeed, 0f);

        // Apply velocity Verlet integration
        Vector2 oldPosition = transform.position;
        Vector2 newPosition = oldPosition + velocity * Time.fixedDeltaTime + 0.5f * acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime;
        transform.position = newPosition;

        Vector2 newVelocity = velocity + 0.5f * (acceleration + gravity * Vector2.down) * Time.fixedDeltaTime;
        velocity = newVelocity;

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpVelocity;
            isGrounded = false;
        }

        // Handle ground detection and correction
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + new Vector3(0f, -0.4f, 0f), new Vector2(0.9f, 0.1f), 0f, LayerMask.GetMask("Ground"));
        if (colliders.Length > 0)
        {
            float distanceToGround = float.MaxValue;
            foreach (Collider2D collider in colliders)
            {
                float colliderDistanceToGround = Mathf.Abs(collider.bounds.max.y - transform.position.y - GetComponent<BoxCollider2D>().size.y / 2);
                if (colliderDistanceToGround < distanceToGround)
                {
                    distanceToGround = colliderDistanceToGround;
                }
            }

            if (distanceToGround < distanceToGroundThreshold)
            {
                isGrounded = true;
                velocity.y = 0f;
                Vector2 snapPosition = new Vector2(transform.position.x, transform.position.y + distanceToGround - GetComponent<BoxCollider2D>().size.y / 2);
                transform.position = snapPosition;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CalculateJumpVelocity()
    {
        float gravity = -Physics2D.gravity.y;
        float jumpTime = Mathf.Sqrt(2f * jumpHeight / gravity);
        jumpVelocity = gravity * jumpTime;
        float timeToPeak = jumpTime / 2f;
        float peakVelocity = gravity * timeToPeak;
        totalJumpTime = jumpTime + timeToPeak * 2f;
        timeToJumpApex = timeToPeak;
    }
}