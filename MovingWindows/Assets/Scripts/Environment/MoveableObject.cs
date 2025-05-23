using UnityEngine;
using UnityEngine.InputSystem;


public class MoveableObject : InteractableObject
{
    Rigidbody2D rb;
    [SerializeField] PlayerInput input;

    bool isMoving;
    bool offsetTransform = true;
    [SerializeField] float offsetAmount;

    [SerializeField] Player2D player;
    [SerializeField] PlayerCharacter2D playerCharacter2D;

    float targetVelocityX;
    float targetVelocityY;

    float playerVelocityY;

    float velX;
    float velY;

    Vector2 vel;

    Vector2 offset;

    [SerializeField] float minDst;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (input.actions["Interact"].WasPressedThisFrame())
            {
                isMoving = !isMoving;
                //rb.simulated = false;
                offset = transform.position - player.transform.position + Vector3.up * offsetAmount;
            }
        }

        //if (isMoving)
        //{
        //    if (offsetTransform)
        //    {
        //        offsetTransform = false;
        //        offset = transform.position - player.transform.position;
        //        rb.gravityScale = 0;

        //        rb.linearVelocity += Vector2.up * offsetAmount;
        //    }

        //    //transform.position = player.transform.position + (Vector3)offset;

        //    if (player.controller.collisions.above || player.controller.collisions.below)
        //    {
        //        playerVelocityY = 0f;
        //    }
        //    else
        //    {
        //        playerVelocityY = player.velocity.y;
        //    }


        //    //targetVelocityY = Mathf.SmoothDamp(rb.linearVelocity.y, playerVelocityY, ref velY, 0.1f);
        //    //targetVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, player.velocity.x, ref velX, 0.1f);


        //    //rb.linearVelocity = new Vector2(targetVelocityX, targetVelocityY);

        //    rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, player.velocity, ref vel, 0.1f);
        //}
        //else
        //{
        //    rb.gravityScale = 1;
        //    offsetTransform = true;
        //}

    }

    void FixedUpdate()
    {
  
        if (isMoving)
        {
            //rb.linearVelocity = player.velocity;
            Vector2 currentPos = rb.position;
            Vector2 targetPos = (Vector2)player.transform.position + offset;
            Vector2 smoothedPos = Vector2.Lerp(currentPos, targetPos, 0.2f); // adjust factor
            playerCharacter2D.collisions.isMovingObject = true;



            float dist = (currentPos - (Vector2)player.transform.position).magnitude;
            if (dist < minDst && input.actions["Move"].ReadValue<Vector2>() != Vector2.zero)
            {
                rb.MovePosition(player.transform.position + (transform.position - player.transform.position));
            }

            rb.MovePosition(smoothedPos);

            //rb.MovePosition(targetPos);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
            playerCharacter2D.collisions.isMovingObject = false;
        }
    }

    public void MoveTransform(Vector2 moveAmount)
    {
        rb.MovePosition(moveAmount);
    }
    
}
