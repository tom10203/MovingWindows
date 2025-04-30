using UnityEngine;

[RequireComponent (typeof(PlayerCharacter2D))]
[RequireComponent (typeof(CastPortal))]
public class CheckPortalBounds : MonoBehaviour
{

    private CastPortal portalBounds;
    private PlayerCharacter2D playerCharacter;
    private BoxCollider2D boxCollider;

    public PortalInfo portalInfo; 
    private void Start()
    {
        playerCharacter = GetComponent<PlayerCharacter2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        portalBounds = GetComponent<CastPortal>();
    }
    void Update()
    {
        portalInfo.Reset();
        if (CanCheck())
        {
            CheckPortals();
        }
    }

    private bool CanCheck()
    {
        if (portalBounds.portals[0] != null && portalBounds.portals[1] != null)
        {
            return true;
        }
        return false;
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

    void CheckPortals()
    {
        for (int i = 0; i < portalBounds.portals.Length; i++)
        {
            {
                Transform portal = portalBounds.portals[i];

                if (CheckBounds(portal, boxCollider))
                {
                    portalInfo.inPortal = true;
                    portalInfo.currentPortal = portal;
                    portalInfo.targetPortal = portalBounds.portals[(i + 1) % 2];
                    return;
                }
            }
        }
    }

    public void SwapPlayerPosition()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        Transform currentPortal = portalInfo.currentPortal;
        Transform targetPortal = portalInfo.targetPortal;

        Vector3 offset = targetPortal.position - currentPortal.position;
        transform.position = transform.position + offset;
        //}
    }
    
    public struct PortalInfo
    {
        public bool inPortal;
        public Transform currentPortal;
        public Transform targetPortal;

        public void Reset()
        {
            inPortal = false;
            currentPortal = null;
            targetPortal = null;
        }
    }
}
