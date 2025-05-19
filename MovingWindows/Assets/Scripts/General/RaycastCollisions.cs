using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class RaycastCollisions : MonoBehaviour
{
    BoxCollider2D _collider;
    [SerializeField] int noOfRays = 4;
    [SerializeField] LayerMask collisionMask;
    float skinWidth = 0.01f;
    RaycastOrigins raycastOrigins = new RaycastOrigins();

    float raySpaceVertical;
    float raySpaceHorizontal;

    public Vector3 velocity;
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
    }


    public void PerformCollisionCheck(ref Vector3 velocity)
    {
        UpdateBounds();

        CheckHorizontalCollisions(ref velocity);

        CheckVerticalCollisions(ref velocity);
    }

    void CheckHorizontalCollisions(ref Vector3 velocity)
    {
        float xDir = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        Vector3 startOrigin = xDir < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

        for (int i = 0; i < noOfRays; ++i)
        {
            Vector3 rayOrigin = startOrigin + Vector3.up * i * raySpaceHorizontal;
            Debug.DrawRay(rayOrigin, Vector3.right * xDir * rayLength, Color.yellow);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * xDir * rayLength, rayLength, collisionMask);

            if (hit)
            {
                rayLength = hit.distance;
                velocity.x = (hit.distance - skinWidth) * xDir;
            }
        }

    }

    void CheckVerticalCollisions(ref Vector3 velocity)
    {
        float yDir = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        Vector3 startOrigin = yDir < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

        for (int i = 0; i < noOfRays; ++i)
        {
            Vector3 rayOrigin = startOrigin + Vector3.right * i * raySpaceVertical * velocity.x;
            Debug.DrawRay(rayOrigin, Vector3.up * yDir * rayLength, Color.yellow);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.up * yDir * rayLength, rayLength, collisionMask);

            if (hit)
            {

                rayLength = hit.distance;
                velocity.y = (hit.distance - skinWidth) * yDir;
            }
        }
    }

    void UpdateBounds()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(-skinWidth * 2);

        raycastOrigins.bottomLeft  = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft     = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight    = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(-skinWidth * 2);

        raySpaceVertical = bounds.size.x / (noOfRays - 1);
        raySpaceHorizontal   = bounds.size.y / (noOfRays - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 bottomLeft, bottomRight;
        public Vector2 topLeft, topRight;
    }
}
