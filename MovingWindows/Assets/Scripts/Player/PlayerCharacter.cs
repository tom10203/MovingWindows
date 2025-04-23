using UnityEngine;
public struct PlayerBounds
{
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
    public Vector2 topLeft;
    public Vector2 topRight;
}

public struct PlayerCollisions
{
    public bool above, below;
    public bool left, right;

    public void Reset()
    {
        above = below = false;
        right = left = false;
    }
}


[RequireComponent(typeof(Player))]
public class PlayerCharacter : MonoBehaviour
{
    private float skinWidth = 0.01f;
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private int raysHorizontal;
    [SerializeField] private int raysVertical;
    [SerializeField] private float maxClimbAngle = 70f;

    [SerializeField] LayerMask layerMask;

    public PlayerBounds playerBounds = new PlayerBounds();
    public PlayerCollisions playerCollisions = new PlayerCollisions();

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    public void Move(Vector3 velocity)
    {
        UpdateBounds();

        playerCollisions.Reset();

        CheckCollisionsHorizontal(ref velocity);
        CheckCollisionsVertical(ref velocity);
        



        transform.Translate(velocity);
    }

    private void UpdateBounds()
    {
        Bounds boxColliderBounds = boxCollider.bounds;
        boxColliderBounds.Expand(-skinWidth * 2);
        playerBounds.bottomLeft  = new Vector2(boxColliderBounds.min.x, boxColliderBounds.min.y);
        playerBounds.bottomRight = new Vector2(boxColliderBounds.max.x, boxColliderBounds.min.y);
        playerBounds.topLeft     = new Vector2(boxColliderBounds.min.x, boxColliderBounds.max.y);
        playerBounds.topRight     = new Vector2(boxColliderBounds.max.x, boxColliderBounds.max.y);

        horizontalRaySpacing = boxColliderBounds.size.x / (raysHorizontal - 1);
        verticalRaySpacing   = boxColliderBounds.size.y / (raysVertical   - 1);   
    }

    void CheckCollisionsVertical(ref Vector3 velocity)
    {
        int direction = velocity.y <= 0 ? -1 : 1;
        float rayDist = Mathf.Abs(velocity.y) + skinWidth;

        Vector3 startPos = direction < 0 ? playerBounds.bottomLeft : playerBounds.topLeft;

        for (int i = 0; i < raysVertical; i++)
        {
            Vector3 rayStart = startPos + transform.right * horizontalRaySpacing * i;
            Vector3 rayDirection = direction < 0 ? -transform.up : transform.up;

            RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayDist, layerMask);
            Debug.DrawRay(rayStart, rayDirection * rayDist * 10, Color.red);

            if (hit)
            {
                float horizontalVel = Mathf.Abs(velocity.x);

                float angle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 || i == raysVertical - 1)
                {
                    velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * velocity.x;
                    velocity.y =  Mathf.Sin(angle * Mathf.Deg2Rad) * velocity.x;
                }

                rayDist = hit.distance;

                velocity.y = (rayDist - skinWidth) * direction;
                playerCollisions.above = direction == 1;
                playerCollisions.below = direction == -1;
            }

        }

    }

    void CheckCollisionsHorizontal(ref Vector3 velocity)
    {
        int direction = velocity.x <= 0 ? -1 : 1;
        float rayDist = Mathf.Abs(velocity.x) + skinWidth;

        Vector3 startPos = direction < 0 ? playerBounds.bottomLeft : playerBounds.bottomRight;


        for (int i = 0; i < raysHorizontal; i++)
        {
            Vector3 rayStart = startPos + transform.up * horizontalRaySpacing * i;
            Vector3 rayDirection = direction < 0 ? -transform.right : transform.right;

            RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayDist, layerMask);
            Debug.DrawRay(rayStart, rayDirection * rayDist * 10, Color.red);

            
            if (hit)
            {
                float angle = Vector2.Angle(Vector2.up, hit.normal);

                if (i == 0 && angle < maxClimbAngle)
                {
                    float storedVelocity = Mathf.Abs(velocity.y);
                    float slopeVelocity  = Mathf.Abs(velocity.x) * Mathf.Sin(angle * Mathf.Deg2Rad);

                    if (storedVelocity > slopeVelocity)
                    {
                        velocity.y  = storedVelocity;
                    }
                    else
                    {
                        velocity.y = slopeVelocity;
                        playerCollisions.below = true;
                    }
                    velocity.x = Mathf.Abs(velocity.x) * Mathf.Cos(angle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    return;
                }

                rayDist = hit.distance;
                velocity.x = (rayDist - skinWidth) * direction;

                playerCollisions.left = direction == -1;
                playerCollisions.right = direction == 1;
                
            }
        }

  
            
     
    }
}
