using UnityEngine;

public class Portal : MonoBehaviour
{
    public Bounds bounds = new Bounds();
    void Start()
    {
        bounds = GetComponent<Renderer>().bounds;        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .5f);
        Gizmos.DrawCube(bounds.center, bounds.size);
    }


}
