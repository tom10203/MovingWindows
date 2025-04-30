using UnityEngine;

public class CastPortal : MonoBehaviour
{
    [SerializeField] private Material[] materials;
    [SerializeField] private GameObject portalParent;
    [SerializeField] private GameObject portal;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Camera cam;
    [SerializeField] private float angleThreshold = 1f;
    [HideInInspector] public Transform[] portals = new Transform[2];

    private int portalsInScene;

    private float halfPortalWidth; // Portals currently square so no need for height
    private int noOfPortals;

    private void Start()
    {
        halfPortalWidth = portal.GetComponent<Renderer>().bounds.extents.x;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 direction = mousePos - transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude + halfPortalWidth, collisionMask);

            if (hit)
            {
                // check to see if hit is side wall
                // As side walls all have custom physics shapes, will check to see if angle between hit normal and vector2.left/right is below a threshold

                Vector2 wallDirection = (Vector2.Dot(Vector2.left, hit.normal) > 0 ? Vector2.left : Vector2.right);
                float angle = Vector2.Angle(wallDirection, hit.normal);

                if (angle < angleThreshold) // Have hit a side wall
                {
                    // Check to see if portal will hit floor/ cieling -> adjust starting pos if so

                    Vector3 portalPosition = hit.point + hit.normal * halfPortalWidth;

                    RaycastHit2D downHit = Physics2D.Raycast(portalPosition, Vector2.down, halfPortalWidth, collisionMask);
                    if (downHit)
                    {
                        float adjustmentDistance = halfPortalWidth - downHit.distance;
                        portalPosition.y += adjustmentDistance;
                    }

                    RaycastHit2D upHit = Physics2D.Raycast(portalPosition, Vector2.up, halfPortalWidth, collisionMask);
                    Debug.DrawRay(portalPosition, Vector2.up, Color.blue, 5f);
                    if (upHit)
                    {
                        if (downHit)
                        {
                            Debug.Log($"Cant cast portal due to insufficient space");
                            return;
                        }

                        float adjustmentDistance = halfPortalWidth - upHit.distance;
                        portalPosition.y -= adjustmentDistance;
                    }

                    Debug.DrawRay(hit.point, hit.normal, Color.yellow, 10f);
                    float distance = hit.distance;
                    
                    Instantiate(portal, portalPosition, Quaternion.Euler(-90, 0, 0));
                }
            }
        }
    }

  
}
