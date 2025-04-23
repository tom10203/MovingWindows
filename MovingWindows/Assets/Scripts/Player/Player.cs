using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public struct CharacterState
{
    public bool grounded;
}

[RequireComponent(typeof(PlayerCharacter))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;

    [SerializeField] private float timeToJumpApex;
    [SerializeField] private float jumpHeight;

    private float gravity;
    private float jumpVelocity;

    [Header("Player Movement Variables")]
    [SerializeField] private float horizontalMovementSpeed = 1f;
    [SerializeField] private float horizontalAcceleration = 0.5f;
    [SerializeField] private float horizontalAccelerationAir = 0.5f;
    [SerializeField] private float horizontalDeceleration = 100f;
    [SerializeField] private float coyoteTime = 0.1f;
    private float horizontalSpeedCurrent = 0f;

    private Vector2 velocity;

    private InputAction _move, _jump;
    

    PlayerCharacter _character;

    float timeSinceHorizontalInputPressed;
    float storedVelocity;
    void Start()
    {
        _character = GetComponent<PlayerCharacter>();

        _input = GetComponent<PlayerInput>();

        _move = _input.actions.FindAction("Move");
        _jump = _input.actions.FindAction("Jump");

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;   
    }

    void Update()
    {
        //if (_character.playerCollisions.below || _character.playerCollisions.above)
        //{
        //    velocity.y = -0.05f;
        //}

        velocity += Vector2.up * gravity * Time.deltaTime;

        Vector2 move = _move.ReadValue<Vector2>();
        float xMovement = move.x;
        HandleHorizontalInput(ref velocity, xMovement);

        bool jump = _jump.WasPerformedThisFrame();
        if (jump && _character.playerCollisions.below)
        {
            Debug.Log($"Jump pressed AND ground below");
            velocity.y = jumpVelocity;
        }
   
        _character.Move(velocity * Time.deltaTime);

        if (_character.playerCollisions.below || _character.playerCollisions.above)
        {
            velocity.y = 0f;
        }
    }

    void HandleHorizontalInput(ref Vector2 velocity, float input)
    {
        float acceleration = _character.playerCollisions.below
            ? horizontalAcceleration
            : horizontalAcceleration * horizontalAccelerationAir;

        if (input != 0)
        {
            float currentSpeed = timeSinceHorizontalInputPressed < 0.1f
                ? storedVelocity
                : Mathf.Abs(velocity.x);

            currentSpeed = Mathf.MoveTowards(currentSpeed, horizontalMovementSpeed, acceleration * Time.deltaTime);
            velocity.x = currentSpeed * input;
            storedVelocity = Mathf.Abs(velocity.x);
            timeSinceHorizontalInputPressed = 0f;
        }
        else
        {
            timeSinceHorizontalInputPressed += Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, horizontalDeceleration * Time.deltaTime);
        }
    }
}
