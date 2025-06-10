using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerCharacter2D;

[RequireComponent (typeof(Player2D))]
public class AnimationManager : InPlayScript
{
    Player2D player;
    PlayerInput input;
    [SerializeField] Animator animator;
    [SerializeField] Transform playerSprite;
    [SerializeField] private float walkingThreshold = 0.01f;
    [SerializeField] private Transform dummy;
    private Animator dummyAnimator;
    public bool dummyInScene = true;
    bool getDummyAnimator = true;

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
        if (inPlay)
        {
            Vector3 playerVelocity = player.velocity;
            CollisionInfo collisionInfo = player.controller.collisions;

            float playerVelocityX = playerVelocity.x;

            Vector2 moveInputValue = input.actions["Move"].ReadValue<Vector2>();
            float xDir = moveInputValue.x;

            // check to see if direction of left/right movement changed -> rotate player to look direction if so
            lookDirection = (playerVelocityX == 0 ? lookDirection : (playerVelocityX < 0 ? -1 : 1));
            
            playerSprite.transform.eulerAngles = lookDirection > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);

            if (input.actions["Move"].IsPressed() && collisionInfo.below)
            {
                animState = PlayerAnimState.Walking;
            }

            if (!input.actions["Move"].IsPressed() && collisionInfo.below)
            {
                animState = PlayerAnimState.Idle;
            }

            if (!collisionInfo.below)
            {
                animState = PlayerAnimState.Jumping;
            }

            animator.SetInteger("State", (int)animState);
        }

    }

    enum PlayerAnimState
    {
        Idle = 0,
        Walking = 1,
        Jumping = 2
    }
}
