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
    private float horizontalSpeedCurrent = 0f;

    private Vector2 velocity;

    private InputAction _move, _jump;
    

    PlayerCharacter _character;
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

        velocity += Vector2.up * gravity * Time.deltaTime;

        Vector2 move = _move.ReadValue<Vector2>();
        float xMovement = move.x;
        Debug.Log($"xMovement {xMovement}");
        HandleHorizontalInput(ref velocity, xMovement);

        bool jump = _jump.WasPerformedThisFrame();
        if (jump && _character.playerCollisions.below)
        {
            velocity.y = jumpVelocity;
        }

        _character.Move(velocity * Time.deltaTime);
    }

    void HandleHorizontalInput(ref Vector2 velocity, float input)
    {
        if (input != 0)
        {
            // Move absolute velocity towards target velocity
            // Have velocity move in direction of input
            float tempVel = Mathf.Abs(velocity.x);
            tempVel = Mathf.MoveTowards(tempVel, horizontalMovementSpeed, (_character.playerCollisions.below ? horizontalAcceleration : horizontalAcceleration * horizontalAccelerationAir) * Time.deltaTime);
            velocity.x = tempVel * input;

        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, input * horizontalMovementSpeed, horizontalDeceleration * Time.deltaTime);
        }
    }
}
