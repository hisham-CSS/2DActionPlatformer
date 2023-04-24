using UnityEngine;

public class Controller2D
{
    public Collider2D collider;
    public Rigidbody2D rb;
    public Vector2 velocity;
    public CollisionInfo collisions;
    public float skinWidth;
    public LayerMask collisionMask;

    private const float minMoveDistance = 0.001f;

    private float gravity;
    private float jumpVelocity;
    private float rayCount = 4;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    public Controller2D(BoxCollider2D collider, Rigidbody2D rb, float skinWidth, LayerMask collisionMask)
    {
        this.collider = collider;
        this.rb = rb;
        this.skinWidth = skinWidth;
        this.collisionMask = collisionMask;

        collisions = new CollisionInfo();
        collisions.Reset();

        //CalculateRaySpacing();
        // Calculate ray spacing
        Bounds bounds = collider.bounds;

        horizontalRaySpacing = bounds.size.y / (rayCount - 1);
        verticalRaySpacing = bounds.size.x / (rayCount - 1);
    }

    public void SetGravity(float gravity)
    {
        this.gravity = gravity;
    }

    public void SetJumpVelocity(float jumpVelocity)
    {
        this.jumpVelocity = jumpVelocity;
    }

    public void SetVelocity(Vector2 inputMovement, Vector2 rbVelocity)
    {
        float targetVelocityX = inputMovement.x;
        velocity.x = Mathf.SmoothDamp(rbVelocity.x, targetVelocityX, ref velocity.x, 0.05f);

        if (collisions.below)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
    }

    public void Move(Rigidbody2D rb)
    {
        Vector2 previousPosition = rb.position;
        Vector2 displacement = velocity * Time.fixedDeltaTime;

        if (displacement.magnitude < minMoveDistance) return;

        HorizontalCollisions(ref displacement);
        VerticalCollisions(ref displacement, displacement.y);

        Vector2 newPosition = previousPosition + displacement;

        // Update the velocity vector using the verlet integration scheme
        velocity = (newPosition - previousPosition) / Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    void HorizontalCollisions(ref Vector2 displacement)
    {
        float directionX = Mathf.Sign(displacement.x);
        float rayLength = Mathf.Abs(displacement.x) + skinWidth;

        if (displacement.x == 0) return;

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? GetBottomLeftCorner() : GetBottomRightCorner();
            rayOrigin += Vector2.up * (i * horizontalRaySpacing);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red); // Draw the raycast

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if (hit)
            {
                displacement.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }

        // Ensure rayLength is never greater than the distance the player is trying to move
        rayLength = Mathf.Min(rayLength, Mathf.Abs(displacement.x) + skinWidth);
    }

    void VerticalCollisions(ref Vector2 displacement, float yDisplacement)
    {
        float directionY = Mathf.Sign(yDisplacement);
        float rayLength = Mathf.Abs(yDisplacement) + skinWidth;

        if (yDisplacement == 0) return;

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? GetBottomLeftCorner() : GetTopLeftCorner();
            rayOrigin += Vector2.right * (i * verticalRaySpacing + displacement.x);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red); // Draw the raycast

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            if (hit)
            {
                displacement.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

                if (directionY == -1)
                {
                    // Update the velocity vector based on the elapsed time and the displacement
                    velocity.y = -(displacement.y / Time.fixedDeltaTime);
                }
            }
        }

        // Ensure rayLength is never greater than the distance the player is trying to move
        rayLength = Mathf.Min(rayLength, Mathf.Abs(displacement.y) + skinWidth);
    }

    Vector2 GetBottomLeftCorner()
    {
        return new Vector2(collider.bounds.min.x, collider.bounds.min.y);
    }

    Vector2 GetBottomRightCorner()
    {
        return new Vector2(collider.bounds.max.x, collider.bounds.min.y);
    }

    Vector2 GetTopLeftCorner()
    {
        return new Vector2(collider.bounds.min.x, collider.bounds.max.y);
    }

    public void Jump()
    {
        velocity.y = jumpVelocity;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
