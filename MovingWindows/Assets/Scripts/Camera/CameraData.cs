using System.Drawing;
using UnityEngine;

[ExecuteInEditMode]
public class CameraData : MonoBehaviour
{
    public float lookAheadDst;
    public float verticalOffset;
    public Vector2 focusAreaSize;
    public Vector2 focusPoint;

    public bool moveCamera = false;
    public bool setFocusAreaRelativeToPlayer = true;

    [SerializeField] Transform player;
    [SerializeField] Camera cam;

    [SerializeField] Transform focusTransform;
    BoxCollider2D targetCollider;
    CameraFollow camFollow;

    Bounds colliderBounds;
    Bounds focusArea;

    public Vector2 focusAreaOffset;
    Vector2 camOffset;

    public bool activateFocusArea;

    bool isActive;

    private void Start()
    {
        
        camFollow = cam.transform.GetComponent<CameraFollow>();
        SetFocusArea();
        camOffset = focusTransform.position - focusArea.center;
        targetCollider = player.GetComponent<BoxCollider2D>();
        
    }

    private void Update()
    {
        if (isActive)
        {
            
        }

        if (CheckOverlap(targetCollider.bounds, focusArea))
        {
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;


        // Draw a wire cube (square if flat on one axis)
        Gizmos.DrawWireCube(transform.position + (Vector3)focusAreaOffset, new Vector3(focusAreaSize.x, focusAreaSize.y, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isActive = true;

            Debug.Log($"player entered camera data object");
            SetFocusArea();
            camFollow.focusArea.UpdateNewFocusArea(focusArea);
            camFollow.offset = camOffset;

            //camFollow.focusArea.UpdateFocusAreaTransform()
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 3)
        {
            Debug.Log($"Player leaving cam data area");
            camFollow.focusArea.UpdateFocusAreaPlayer(targetCollider.bounds);
            camOffset = Vector3.up * camFollow.verticalOffset;
            camFollow.offset = camOffset;
        }
    }

    void SetFocusArea()
    {
        Vector3 boundsCenter = new Vector3(transform.position.x, transform.position.y, player.position.z) + (Vector3)focusAreaOffset;
        focusArea = new Bounds(boundsCenter, focusAreaSize);
    }

    bool CheckOverlap(Bounds player, Bounds focusArea)
    {
        return (player.min.x >= focusArea.min.x && player.max.x <= focusArea.max.x) && (player.min.y >= focusArea.min.y && player.max.y <= focusArea.max.y);    
    }

}
