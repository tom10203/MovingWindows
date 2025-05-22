using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PortalManager : InPlayScript
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Material[] materials;
    [SerializeField] private Transform player;

    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera textureCam;
    [SerializeField] private float angleThreshold = 1f;
    [SerializeField] private PlayerInput input;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject spellPrefab;
   

    [HideInInspector] public GameObject[] portals;

    private float halfPortalWidth;
    private float halfPortalHeight;
    public int noOfPortalsInScene;
    private BoxCollider2D boxCollider;

    [HideInInspector] public PortalInfo portalInfo;

    bool setPortalBounds = true;
    bool swapPositions = true;

    VirtualBounds vb = new VirtualBounds();
    CastPortal castPortal;

    bool setTextureCamPos = true;
    void Start()
    {
        halfPortalWidth = portalPrefab.GetComponent<Renderer>().bounds.extents.x;
        halfPortalHeight = portalPrefab.GetComponent<Renderer>().bounds.extents.y;
        boxCollider = player.GetComponent<BoxCollider2D>();

        portals = new GameObject[2];
        portalInfo = new PortalInfo();
        castPortal = FindFirstObjectByType<CastPortal>();

    }

    // Update is called once per frame
    void Update()
    {
        if (inPlay)
        {


            portalInfo.Reset();


            if (input.actions["Attack"].WasPerformedThisFrame() && noOfPortalsInScene < 2)
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                castPortal.PerformCast(mousePos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ResetPortals();
            }

            if (noOfPortalsInScene == 2)
            {
                if (setTextureCamPos)
                {
                    setTextureCamPos = false;
                    SetTextureCamPosition();
                }

                SwapTextures();

                CheckPortals();

                if (setPortalBounds)
                {
                    setPortalBounds = false;
                    SetPortalBounds();
                }

                if (portalInfo.inPortal)
                {
                    if (swapPositions)
                    {
                        SwapPlayerPosition();
                        swapPositions = false;
                    }
                }
                else
                {
                    swapPositions = true;
                }

            }
            else
            {
                setTextureCamPos = true;
            }
        }

    }

    //private void OnDrawGizmos()
    //{
    //    if (noOfPortalsInScene == 2)
    //    {
    //        Gizmos.color = new Color(0, 0, 1, 0.5f);
    //        Gizmos.DrawCube(portalInfo.portalBounds.center, portalInfo.portalBounds.size);
    //    }
    //}
    void SetTextureCamPosition()
    {
        Vector3 pos1 = portals[0].transform.position;
        Vector3 pos2 = portals[1].transform.position;

        Vector3 midPoint = (pos1 + pos2) / 2;

        textureCam.transform.position = new Vector3(midPoint.x, midPoint.y, textureCam.transform.position.z);
    }

    Bounds CreateVirtualBounds(Vector3 boundsCenter)
    {
        Bounds virtualBounds = new Bounds(boundsCenter, new Vector3(halfPortalWidth * 2, halfPortalHeight * 2, 0));
        return virtualBounds;
    }

    public bool CheckVirtualOverlap(Vector3 hitPoint)
    {
        if (portals[0] != null && portals[1] == null)
        {
            Debug.Log($"CHecking virtual overlap");
            Bounds virtualBounds = CreateVirtualBounds(hitPoint);
            vb.virtualBounds = virtualBounds;
            Transform portal = portals[0].transform;
            Bounds portalBounds = portal.GetComponent<Renderer>().bounds;
            vb.portalBounds = portalBounds;


            if ((virtualBounds.max.x >= portalBounds.min.x && virtualBounds.min.x <= portalBounds.max.x)
            && (virtualBounds.max.y >= portalBounds.min.y && virtualBounds.min.y <= portalBounds.max.y))
            {
                Debug.Log($"Intersection found");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    struct VirtualBounds
    {
        public Bounds portalBounds;
        public Bounds virtualBounds;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(vb.virtualBounds.center, vb.virtualBounds.size);
    //    Gizmos.DrawCube(vb.portalBounds.center, vb.portalBounds.size);
    //}

    void SetPortalBounds()
    {
        // only called when both portals are cast

        float minX = Mathf.Min(portals[0].GetComponent<Portal>().bounds.min.x, portals[1].GetComponent<Portal>().bounds.min.x, boxCollider.bounds.min.x);
        float maxX = Mathf.Max(portals[0].GetComponent<Portal>().bounds.max.x, portals[1].GetComponent<Portal>().bounds.max.x, boxCollider.bounds.max.x);

        float minY = Mathf.Min(portals[0].GetComponent<Portal>().bounds.min.y, portals[1].GetComponent<Portal>().bounds.min.y, boxCollider.bounds.min.y);
        float maxY = Mathf.Max(portals[0].GetComponent<Portal>().bounds.max.y, portals[1].GetComponent<Portal>().bounds.max.y, boxCollider.bounds.max.y);

        float xDst = (maxX - minX);
        float yDst = (maxY - minY);

        float centreX = (minX + maxX) / 2;
        float centreY = (minY + maxY) / 2;

        portalInfo.portalBounds = new Bounds (new Vector2 (centreX, centreY), new Vector2(xDst, yDst));
    }

    public void CastPortal(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (noOfPortalsInScene < 2)
        {

            float dot = Vector3.Dot(hitNormal, Vector3.up);
            float extents = Mathf.Abs(dot) > 0.9f ? halfPortalHeight : halfPortalWidth;

            Vector3 portalPosition = hitPoint + hitNormal * extents;
            portalPosition = AdjustPosition(portalPosition, hitNormal);
            portalPosition += CheckForLedge(portalPosition, hitNormal);

            InstantiatePortal(portalPosition);
        }

    }

    void AdjustPositionTest(Vector3 hitPoint)
    {
        // GetBounds of where portal will be
        // Boxcast where portalwill be
        // see results

        Bounds prePortalBounds = new Bounds(hitPoint, new Vector3(halfPortalWidth * 2, halfPortalWidth * 2, 0));

        RaycastHit2D[] colliders = Physics2D.BoxCastAll(hitPoint, new Vector2(halfPortalWidth * 2, halfPortalHeight * 2), 0f, Vector2.zero, collisionMask);

        Vector3 min = prePortalBounds.min;
        Vector3 max = prePortalBounds.max;

        Vector3Int minCell = tilemap.WorldToCell(min);
        Vector3Int maxCell = tilemap.WorldToCell(max);

        float maxColliderX, maxColliderY = float.MinValue;
        float minColliderX, minColliderY = float.MaxValue;

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    Vector3 test = tilemap.CellToWorld(pos);
                    Debug.Log($"test {test}");
                    Debug.Log($"Tile at {pos} is {tile.name}");
                }
            }
        }

        //if (colliders.Length > 0)
        //{
        //    Debug.Log($"Colliders length {colliders.Length}");
        //    Debug.Log(colliders[0].collider.gameObject.transform.position);
        //}
    }

    Vector3 AdjustPosition(Vector3 spawnPoint, Vector3 normal)
    {
        // normal relates to hit normal, therefore want raycast direction to be perpendicular to normal
        // send out 4 raycasts at top, bottm / left, right of portal and adjust accordingly

        float dot = Vector3.Dot(Vector3.up, normal);
        Vector3 moveDirection = Vector3.zero;
        if (Mathf.Abs(dot) > 0.9f)
        {
            moveDirection = Vector3.left;
        }
        else
        {
            moveDirection = Vector3.up;
        }

        //Debug.Log(moveDirection);

        float extents1 = moveDirection == Vector3.left ? halfPortalHeight : halfPortalWidth ;
        float extents2 = moveDirection == Vector3.left ? halfPortalWidth  : halfPortalHeight;
        

        for (int i = -1; i < 2; i+=2) 
        {
            float raycastDistance = extents2;

            for (int j = -1; j < 2; j += 2)
            {
                Vector3 origin = spawnPoint + normal * j * (extents1 - 0.01f);
                Vector3 direction = moveDirection * i * raycastDistance;
                Debug.DrawRay(origin, direction, Color.red, 10f);
                RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, direction, direction.magnitude, collisionMask);
                if (raycastHit2D)
                {
                    raycastDistance = 0;
                    float distanceToMove = extents2 - raycastHit2D.distance;
                    spawnPoint += - moveDirection * i * distanceToMove;
                }
            }
        }
        return spawnPoint;

    }

    Vector3 CheckForLedge(Vector3 spwanPoint, Vector3 normal)
    {
        // Cast rays from top, bottom / left, right of portal based on normal direction
        // if one ray does not hit -> cast ray towards other raycast point and move portal that distance

        float dot = Vector3.Dot(normal, Vector3.up);
        float extents1 = Mathf.Abs(dot) > 0.9f ? halfPortalWidth  : halfPortalHeight;
        float extents2 = Mathf.Abs(dot) > 0.9f ? halfPortalHeight : halfPortalWidth ;
        Vector3 movement = Mathf.Abs(dot) > 0.9f ? Vector3.left : Vector3.up;

        for (int i = -1; i < 2; i+= 2)
        {
            Vector3 origin = spwanPoint - normal * (extents2 - 0.01f) + movement * i * extents1;
            origin.z = 1;
            Vector3 rayDirection = -normal * 0.01f * 2;

            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, rayDirection.magnitude, collisionMask);
            Debug.DrawRay(origin, rayDirection, Color.green, 10f);

            if (!hit)
            {
                //Debug.Log($"Ledge found");
                Vector3 newOrigin = origin + rayDirection;
                Vector3 newDirection = movement * -i * extents1 * 2;
                RaycastHit2D adjustmentHit = Physics2D.Raycast(newOrigin, newDirection, newDirection.magnitude, collisionMask);
                
                if (adjustmentHit)
                {
                    float distanceToMove = adjustmentHit.distance;
                    return newDirection * distanceToMove;
                }
            }
        }

        return Vector3.zero;
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
        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i] != null)
            {
                UncastPortal(portals[i]);
            }
        }
        Destroy(portals[0]);
        Destroy(portals[1]);

        portals[0] = null;
        portals[1] = null;
        noOfPortalsInScene = 0;

        setPortalBounds = true;
        portalInfo.portalBounds = new Bounds();
    }

    void UncastPortal(GameObject portal)
    {
        Vector3 portalPosition = portal.transform.position;
        Vector3 playerPosition = player.transform.position;

        List<Vector3> points = new List<Vector3>();
        points.Add(playerPosition);

        GameObject spell = Instantiate(spellPrefab, portalPosition, Quaternion.identity);
        SpellCast spellCast = spell.GetComponent<SpellCast>();

        spellCast.points = points.ToArray();
        spellCast.castPortal = false;
        spellCast.moveSpeed = spellCast.moveSpeed * 1.5f;
    }

    void SwapTextures()
    {
        Vector3 screenSpace1 = cam.WorldToViewportPoint(portals[0].transform.position);
        Vector3 screenSpace2 = cam.WorldToViewportPoint(portals[1].transform.position);

        Vector3 camSpace1 = cam.WorldToViewportPoint(cam.transform.position);
        Vector3 camSpace2 = cam.WorldToViewportPoint(textureCam.transform.position);


        Vector3 offset = screenSpace1 - screenSpace2;
        Vector3 camTransformOffset = camSpace1 - camSpace2;

        portals[0].GetComponent<Renderer>().material.SetVector("_Offset", -offset + camTransformOffset);
        portals[1].GetComponent<Renderer>().material.SetVector("_Offset", offset  + camTransformOffset);
        
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
