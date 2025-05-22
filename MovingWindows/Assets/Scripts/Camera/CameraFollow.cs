using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.GameCenter;


public class CameraFollow : InPlayScript
{
    public Transform targetTransform;
    private Player2D target;
    private BoxCollider2D targetCollider;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    public FocusArea focusArea = new FocusArea();

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

    Vector3 vel1;
    RaycastCollisions raycastCollisions;

    public float timeScale = 0.5f;

    bool followFocusArea = false;
    bool checkForStartFollow = true;

    public Vector2 offset;
    public Vector2 focusAreaOffset;

    Bounds startFocusArea;

    

    void Start()
    {
        target = targetTransform.GetComponent<Player2D>();
        targetCollider = targetTransform.GetComponent<BoxCollider2D>();
        raycastCollisions = GetComponent<RaycastCollisions>();

        focusArea.smoothTime = .2f;
        focusArea.playerFocusAreaSize = focusAreaSize;
        focusArea.UpdateFocusAreaPlayer(targetCollider.bounds);

        verticalOffset = transform.position.y - targetTransform.position.y;
    }

    private void Update()
    {
        if (checkForStartFollow)
        {
            if (targetTransform.position.x >= transform.position.x)
            {
                followFocusArea = true;
                checkForStartFollow = false;
            }
        }
    }
    void LateUpdate()
    {
        
        //Time.timeScale = timeScale;

        // Below is to adjust focus area to center on portals when 2 portals are in scene
        //=====================================================================================
        //if (portalManager.noOfPortalsInScene == 2)
        //{
        //    if (updateAreaPortals && portalManager.portalInfo.portalBounds.center != Vector3.zero)
        //    {
        //        focusArea.UpdateFocusAreaPortals(portalManager.portalInfo.portalBounds);
        //        updateAreaPlayer = true;
        //        updateAreaPortals = false;
        //    }
        //}
        //else
        //{

        //    if (updateAreaPlayer)
        //    {
        //        focusArea.UpdateFocusAreaPlayer(targetCollider.bounds, focusAreaSize);
        //        updateAreaPlayer = false;
        //        updateAreaPortals = true;
        //    }
        //}
        //=====================================================================================

        
        if (inPlay)
        {
           
            focusArea.UpdateFocusArea(targetCollider.bounds);


            //focusPosition = Vector2.SmoothDamp(focusPosition, focusArea.centre + Vector2.up * verticalOffset, ref vel, verticalSmoothTime);
            if (followFocusArea)
            {
                //focusPosition = focusArea.centre + offset;
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

                focusPosition.x += currentLookAheadX;

                //focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);


                // Add raycast logic here
                //Vector3 targetPosition = Vector3.SmoothDamp(transform.position, (Vector3)focusPosition + Vector3.forward * -24.34267f, ref vel1, 0.2f);

                //Vector3 velocity = targetPosition - transform.position;
                //Vector3 refVelocity = velocity;

                //raycastCollisions.PerformCollisionCheck(ref velocity);

                //if (refVelocity != velocity)
                //{
                //    currentLookAheadX = oldCurrentLookAheadX;
                //}

                //focusPosition += Vector2.right * currentLookAheadX;

                //transform.position += velocity;

                transform.position = Vector3.SmoothDamp(transform.position, (Vector3)focusPosition + Vector3.forward * -24.34267f, ref vel1, 0.2f);
            }
            //transform.position = focusPosition;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, new Vector2(focusArea.right - focusArea.left, focusArea.top - focusArea.bottom));
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = UnityEngine.Color.red;


    //    // Draw a wire cube (square if flat on one axis)
    //    Gizmos.DrawWireCube(transform.position + (Vector3)focusAreaOffset, new Vector3(focusAreaSize.x, focusAreaSize.y, 0));
    //}


    [System.Serializable]
    public class FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        public float left, right;
        public float top, bottom;
        Vector2 smoothVelocity;
        public float smoothTime;
        public Vector2 playerFocusAreaSize;
        public bool updateFocusArea;

        //public FocusArea(Bounds targetBounds, Vector2 size)
        //{
        //    left = targetBounds.center.x - size.x / 2;
        //    right = targetBounds.center.x + size.x / 2;
        //    bottom = targetBounds.min.y;
        //    top = targetBounds.min.y + size.y;

        //    velocity = Vector2.zero;
        //    smoothVelocity = Vector2.zero;
        //    centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        //    smoothTime = .2f;
        //}

        //public FocusArea()
        //{
        //    centre = Vector2.zero;
        //    velocity = Vector2.zero;
        //    left = right = 0;
        //    top = bottom = 0;
        //    smoothVelocity = Vector2.zero;
        //    smoothTime = .2f;
        //}
   

        public void UpdateFocusAreaPlayer(Bounds targetBounds)
        {
            left = targetBounds.center.x - playerFocusAreaSize.x / 2;
            right = targetBounds.center.x + playerFocusAreaSize.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + playerFocusAreaSize.y;

            velocity = Vector2.zero;
            //centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }



        public void UpdateNewFocusArea(Bounds newFocusArea)
        {
            left = newFocusArea.center.x - newFocusArea.size.x / 2;
            right = newFocusArea.center.x + newFocusArea.size.x / 2;
            top = newFocusArea.center.y + newFocusArea.size.y / 2;
            bottom = newFocusArea.center.y - newFocusArea.size.y / 2;

            velocity = Vector2.zero;
            //centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }



        public void UpdateFocusArea(Bounds targetBounds)
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
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
            
        }
    }

}
