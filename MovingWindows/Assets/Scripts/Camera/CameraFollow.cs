using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform targetTransform;
    private Player2D target;
    private BoxCollider2D targetCollider;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;
    Vector2 vel;
    Vector2 focusPosition;

    bool lookAheadStopped;

    [SerializeField] PortalManager portalManager;

    bool updateAreaPortals = true;
    bool updateAreaPlayer = false;

    void Start()
    {
        target = targetTransform.GetComponent<Player2D>();
        targetCollider = targetTransform.GetComponent<BoxCollider2D>();

        focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);

        focusPosition = focusArea.centre + Vector2.up * verticalOffset;
    }

    void LateUpdate()
    {
        if (portalManager.noOfPortalsInScene == 2)
        {
            if (updateAreaPortals && portalManager.portalInfo.portalBounds.center != Vector3.zero)
            {
                focusArea.UpdateFocusAreaPortals(portalManager.portalInfo.portalBounds);
                updateAreaPlayer = true;
                updateAreaPortals = false;
            }
        }
        else
        {

            if (updateAreaPlayer)
            {
                focusArea.UpdateFocusAreaPlayer(targetCollider.bounds, focusAreaSize);
                updateAreaPlayer = false;
                updateAreaPortals = true;
            }

            focusArea.Update(targetCollider.bounds);

            //focusPosition = Vector2.SmoothDamp(focusPosition, focusArea.centre + Vector2.up * verticalOffset, ref vel, verticalSmoothTime);
            focusPosition = focusArea.centre + Vector2.up * verticalOffset;

            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
                float targetInputX = target.playerInput.actions["Move"].ReadValue<Vector2>().x;
                if (Mathf.Sign(targetInputX) == Mathf.Sign(focusArea.velocity.x) && targetInputX != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                    }
                }
            }


            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
            focusPosition += Vector2.right * currentLookAheadX;
            transform.position = (Vector3)focusPosition + Vector3.forward * -24.34267f;
            transform.position = focusPosition;

        }



      
        
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, new Vector2 (focusArea.right - focusArea.left, focusArea.top - focusArea.bottom));
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        public float left, right;
        public float top, bottom;
        Vector2 smoothVelocity;
        public float smoothTime;


        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            smoothVelocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            smoothTime = 0.2f;
        }

        public void UpdateFocusAreaPlayer(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            //centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void UpdateFocusAreaPortals(Bounds newFocusArea)
        {
            left = newFocusArea.center.x - newFocusArea.size.x / 2 ;
            right = newFocusArea.center.x + newFocusArea.size.x / 2;
            top = newFocusArea.center.y + newFocusArea.size.y / 2;
            bottom = newFocusArea.center.y - newFocusArea.size.y / 2;

            velocity = Vector2.zero;
            //centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            centre = Vector2.SmoothDamp(centre, new Vector2((left + right) / 2, (top + bottom) / 2), ref smoothVelocity, smoothTime);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

}