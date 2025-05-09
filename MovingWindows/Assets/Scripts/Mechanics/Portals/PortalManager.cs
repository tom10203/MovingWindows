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
    private float halfPortalHeight;
    public int noOfPortalsInScene;
    private BoxCollider2D boxCollider;

    [HideInInspector] public PortalInfo portalInfo;

    bool setPortalBounds = true;

    void Start()
    {
        halfPortalWidth = portalPrefab.GetComponent<Renderer>().bounds.extents.x;
        halfPortalHeight = portalPrefab.GetComponent<Renderer>().bounds.extents.y;
        boxCollider = player.GetComponent<BoxCollider2D>();

        portals = new GameObject[2];
        portalInfo = new PortalInfo();

    }

    // Update is called once per frame
    void Update()
    {
        portalInfo.Reset();

        if (Input.GetMouseButtonDown(1))
        {
            ResetPortals();
        }

        if (noOfPortalsInScene == 2)
        {
            SwapTextures();

            CheckPortals();

            if (setPortalBounds)
            {
                setPortalBounds = false;
                SetPortalBounds();
            }

            Debug.Log($"Portalmanager portalbounds {portalInfo.portalBounds}");
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (noOfPortalsInScene < 2)
            {
                CastPortal();
            }
        }

        

    }

    private void OnDrawGizmos()
    {
        if (noOfPortalsInScene == 2)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(portalInfo.portalBounds.center, portalInfo.portalBounds.size);
        }
    }

    void SetPortalBounds()
    {
        // only called when both portals are cast

        float minX = Mathf.Min(portals[0].GetComponent<Portal>().bounds.min.x, portals[1].GetComponent<Portal>().bounds.min.x, boxCollider.bounds.min.x);
        float maxX = Mathf.Max(portals[0].GetComponent<Portal>().bounds.max.x, portals[1].GetComponent<Portal>().bounds.max.x, boxCollider.bounds.max.x);

        float minY = Mathf.Min(portals[0].GetComponent<Portal>().bounds.min.y, portals[1].GetComponent<Portal>().bounds.min.y, boxCollider.bounds.min.y);
        float maxY = Mathf.Max(portals[0].GetComponent<Portal>().bounds.max.y, portals[1].GetComponent<Portal>().bounds.max.y, boxCollider.bounds.max.y);

        Debug.Log($"Minx {minX}, maxX {maxX}, minY {minY}, maxY {maxY}");
        Debug.Log($"portal manager portal bounds {portals[0].GetComponent<Portal>().bounds}, {portals[1].GetComponent<Portal>().bounds}");

        float xDst = (maxX - minX);
        float yDst = (maxY - minY);

        float centreX = (minX + maxX) / 2;
        float centreY = (minY + maxY) / 2;

        portalInfo.portalBounds = new Bounds (new Vector2 (centreX, centreY), new Vector2(xDst, yDst));
        Debug.Log($"Setting portbounds {portalInfo.portalBounds}");
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
                // Check to see if edge of portal (in case of small floor) will hit floor/ cieling

                Vector3 portalPosition = hit.point + wallDirection * halfPortalWidth;

                RaycastHit2D downHit = Physics2D.Raycast((portalPosition - (Vector3)wallDirection * (halfPortalWidth - 0.01f)), Vector2.down, halfPortalHeight, collisionMask);
                if (downHit)
                {
                    float adjustmentDistance = halfPortalHeight - downHit.distance;
                    portalPosition.y += adjustmentDistance;
                }

                RaycastHit2D upHit = Physics2D.Raycast((portalPosition - (Vector3)wallDirection * (halfPortalWidth - 0.01f)), Vector2.up, halfPortalHeight, collisionMask);
                if (upHit)
                {
                    if (downHit)
                    {
                        Debug.Log($"Cant cast portal due to insufficient space");
                        return;
                    }

                    float adjustmentDistance = halfPortalHeight - upHit.distance;
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
        Vector3 portalLocalPos = newPortal.transform.localPosition;
        portalLocalPos = new Vector3(portalLocalPos.x, portalLocalPos.y, -0.2f);
        newPortal.transform.localPosition = portalLocalPos;

        Material material = (noOfPortalsInScene == 0 ? materials[0] : materials[1]);

        newPortal.GetComponent<Renderer>().material = material;

        portals[noOfPortalsInScene] = newPortal;

        noOfPortalsInScene++;
    }

    void ResetPortals()
    {
        Destroy(portals[0]);
        Destroy(portals[1]);

        portals[0] = null;
        portals[1] = null;
        noOfPortalsInScene = 0;

        setPortalBounds = true;
        portalInfo.portalBounds = new Bounds();
    }

    void SwapTextures()
    {
        Vector3 screenSpace1 = cam.WorldToViewportPoint(portals[0].transform.position);
        Vector3 screenSpace2 = cam.WorldToViewportPoint(portals[1].transform.position);

        Vector3 offset = screenSpace1 - screenSpace2;

        portals[0].GetComponent<Renderer>().material.SetVector("_Offset", -offset);
        portals[1].GetComponent<Renderer>().material.SetVector("_Offset", offset);
    }

    bool CheckBounds(Transform portal, BoxCollider2D playerCollider)
    {

        Bounds portal1Bounds = portal.GetComponent<Renderer>().bounds;
        Bounds playerBounds = playerCollider.bounds;


        if (portal1Bounds.min.y > playerBounds.min.y || portal1Bounds.max.y < playerBounds.max.y)
        {
            return false;
        }

        return (portal1Bounds.min.x > playerBounds.min.x || portal1Bounds.max.x < playerBounds.max.x) ? false : true;
    }

    Vector3 CalculateOffset()
    {
        return (portalInfo.targetPortal.position - portalInfo.currentPortal.position);
    }

    bool CheckBoundsTest(Transform portal, BoxCollider2D playerCollider)
    {

        Bounds portal1Bounds = portal.GetComponent<Renderer>().bounds;
        Bounds playerBounds = playerCollider.bounds;

        if ((portal1Bounds.min.y <= playerBounds.max.y && portal1Bounds.max.y >= playerBounds.min.y) && (portal1Bounds.min.x <= playerBounds.max.x && portal1Bounds.max.x >= playerBounds.min.x)) 
        {
            return true;
        }

        return false;
    }

    void CheckPortals()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            {
                Transform portal = portals[i].transform;

                if (CheckBounds(portal, boxCollider))
                {
                    portalInfo.inPortal = true;
                    portalInfo.currentPortal = portal;
                    portalInfo.targetPortal = portals[(i + 1) % 2].transform;
                    return;
                }
            }
        }
    }

    public void SwapPlayerPosition()
    {
        Transform currentPortal = portalInfo.currentPortal;
        Transform targetPortal = portalInfo.targetPortal;

        Vector3 offset = targetPortal.position - currentPortal.position;
        player.position = player.position + offset;
    }

    public struct PortalInfo
    {
        public bool inPortal;
        public Transform currentPortal;
        public Transform targetPortal;
        public float minX, maxX;
        public float minY, maxY;
        public Vector2 centre;
        public Bounds portalBounds;

        public void Reset()
        {
            inPortal = false;
            currentPortal = null;
            targetPortal = null;

        }
    }
}
