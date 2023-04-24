using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float jumpHeight = 2f;
    public float timeToJumpApex = 0.4f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Controller2D controller;

    public bool isGrounded = false;
    private Vector2 moveDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = new Controller2D(GetComponent<BoxCollider2D>(), rb, 0.01f, groundLayer);

        // Calculate gravity and jump velocity
        float gravity = -(2f * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        float jumpVelocity = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(gravity));

        // Set the controller's gravity and jump velocity
        controller.SetGravity(gravity);
        controller.SetJumpVelocity(jumpVelocity);

        InputManager.Instance.input.Player.Move.performed += ctx => Move(ctx);
        InputManager.Instance.input.Player.Move.canceled += ctx => Move(ctx);
        InputManager.Instance.input.Player.Jump.performed += ctx => Jump(ctx);
    }

    void FixedUpdate()
    {
        controller.collisions.Reset();

        // Calculate the player's movement vector
        Vector2 movement = new Vector2(moveDir.x * moveSpeed, 0.0f);

        // Update the controller's velocity vector
        controller.SetVelocity(movement, rb.velocity);

        // Move the player
        controller.Move(rb);

        Debug.Log(controller.collisions.below);
    }

    void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (controller.collisions.below)
        {
            controller.Jump();
            Debug.Log("Jump");
        }
    }
}

