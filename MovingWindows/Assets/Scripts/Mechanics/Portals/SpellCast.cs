using UnityEngine;

public class SpellCast : MonoBehaviour
{
    public Vector3[] points;
    public bool castPortal;
    public Vector3 hitNormal;
    [SerializeField] public float moveSpeed;
    int currentIndex = 0;

    PortalManager portalManager;
    private void Start()
    {
        portalManager = FindFirstObjectByType<PortalManager>();
    }
    void Update()
    {
       
        Vector3 target = points[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            if (currentIndex == points.Length - 1)
            {
                // Instantiate Portal
                if (castPortal)
                {
                    if (!portalManager.CheckVirtualOverlap(points[points.Length - 1]))
                    {
                        portalManager.CastPortal(points[points.Length - 1], hitNormal);
                    }

                }

                Destroy(gameObject);
            }
            else
            {
                currentIndex++;
            }
        }
    }
}
