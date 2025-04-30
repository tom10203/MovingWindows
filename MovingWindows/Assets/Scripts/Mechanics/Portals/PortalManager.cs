using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Material[] materials;
    [SerializeField] private Transform player;

    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Camera cam;
    [SerializeField] private float angleThreshold = 1f;

    [HideInInspector] public GameObject[] portals;

    private float halfPortalWidth;
    public int noOfPortalsInScene;


    void Start()
    {
        halfPortalWidth = portalPrefab.GetComponent<Renderer>().bounds.extents.x;
        portals = new GameObject[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (noOfPortalsInScene == 2)
        {
            SwapTextures();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (noOfPortalsInScene < 2)
            {
                
                CastPortal();
            }
        }
    }

    void CastPortal()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mousePos - player.position;

        RaycastHit2D hit = Physics2D.Raycast(player.position, direction, direction.magnitude + halfPortalWidth, collisionMask);

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

                InstantiatePortal(portalPosition);
            }
        }
    }

    void InstantiatePortal(Vector3 startPos)
    {
        GameObject newPortal = Instantiate(portalPrefab, startPos, Quaternion.Euler(-90,0,0), transform);

        Material material = (noOfPortalsInScene == 0 ? materials[0] : materials[1]);

        newPortal.GetComponent<Renderer>().material = material;

        portals[noOfPortalsInScene] = newPortal;

        noOfPortalsInScene++;
    }

    void ResetPortals()
    {
        Destroy(portals[1]);
        Destroy(portals[2]);

        portals[0] = null;
        portals[1] = null;
        noOfPortalsInScene = 0;
    }

    void SwapTextures()
    {
        Vector3 screenSpace1 = cam.WorldToViewportPoint(portals[0].transform.position);
        Vector3 screenSpace2 = cam.WorldToViewportPoint(portals[1].transform.position);

        Vector3 offset = screenSpace1 - screenSpace2;

        portals[0].GetComponent<Renderer>().material.SetVector("_Offset", -offset);
        portals[1].GetComponent<Renderer>().material.SetVector("_Offset", offset);
    }
}
