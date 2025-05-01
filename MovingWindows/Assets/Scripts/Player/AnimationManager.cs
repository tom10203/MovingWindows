using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerCharacter2D;

[RequireComponent (typeof(Player2D))]
public class AnimationManager : MonoBehaviour
{
    Player2D player;
    PlayerInput input;
    [SerializeField] Animator animator;
    [SerializeField] Transform playerSprite;
    [SerializeField] private float walkingThreshold = 0.01f;

    private int lookDirection = 1; // 1 == look left, -1 == look right
    PlayerAnimState animState;

    private void Start()
    {
        player = GetComponent<Player2D>();
        input = GetComponent<PlayerInput>();
        animState = new PlayerAnimState();

    }

    private void Update()
    {
        Vector3 playerVelocity = player.velocity;
        CollisionInfo collisionInfo = player.controller.collisions;

        // check to see if direction of left/right movement changed -> rotate player 180 if so
        Vector2 moveInputValue = input.actions["Move"].ReadValue<Vector2>();
        float xDir = moveInputValue.x;

        lookDirection = (xDir == 0 ? lookDirection : (xDir < 0 ? -1 : 1));
        playerSprite.transform.eulerAngles = lookDirection > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);

        if (input.actions["Move"].IsPressed() && collisionInfo.below)
        {
            //Debug.Log($"Move is pressed");
            //animator.SetBool("Walking", true);
            //animator.SetBool("Jumping", false);
            //animator.SetBool("Idle", false);
            animState = PlayerAnimState.Walking;
        }

        if (!input.actions["Move"].IsPressed() && collisionInfo.below)
        {
            //animator.SetBool("Walking", false);
            //animator.SetBool("Jumping", false);
            //animator.SetBool("Idle", true);
            animState = PlayerAnimState.Idle;
        }

        if (!collisionInfo.below)
        {
            //animator.SetBool("Jumping", true);
            //animator.SetBool("Walking", false);
            //animator.SetBool("Idle", false);
            animState = PlayerAnimState.Jumping;
        }

        animator.SetInteger("State", (int)animState);
    }

    enum PlayerAnimState
    {
        Idle = 0,
        Walking = 1,
        Jumping = 2
    }
}
