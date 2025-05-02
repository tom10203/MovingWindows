using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerCharacter2D))]
public class Player2D : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    public float decellerationTimeGrounded = 0.05f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    public Vector3 velocity;
    float velocityXSmoothing;

    public PlayerCharacter2D controller;
    [HideInInspector] public PlayerInput playerInput;

    void Start()
    {
        controller = GetComponent<PlayerCharacter2D>();
        playerInput = GetComponent<PlayerInput>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    void Update()
    {

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
        }

        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (!controller.collisions.below) ? accelerationTimeAirborne : (targetVelocityX == 0 ? decellerationTimeGrounded : accelerationTimeGrounded));
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}