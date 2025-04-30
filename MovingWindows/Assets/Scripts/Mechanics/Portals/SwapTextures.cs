using Unity.VisualScripting;
using UnityEngine;

public class SwapTextures : MonoBehaviour
{

    [SerializeField] private Transform obj1;
    [SerializeField] private Transform obj2;

    [SerializeField] private BoxCollider2D playerCollider;

    [SerializeField] private Camera cam;
    private void Update()
    {
        Vector3 screenSpace1 = cam.WorldToViewportPoint(obj1.position);
        Vector3 screenSpace2 = cam.WorldToViewportPoint(obj2.position);

        Vector3 offset = screenSpace1 - screenSpace2;

        obj1.GetComponent<Renderer>().material.SetVector("_Offset", -offset);
        obj2.GetComponent<Renderer>().material.SetVector("_Offset", offset);


        //Debug.Log(obj1.GetComponent<Renderer>().bounds);
        //Debug.Log(player.GetComponent<Renderer>().bounds);

        Bounds portal1Bounds = obj1.GetComponent<Renderer>().bounds;
        Bounds playerBounds = playerCollider.bounds;

        //Debug.Log($"portal bounds: min, max y {portal1Bounds.min.y}, {portal1Bounds.max.y}, portalbounds: min, max x {portal1Bounds.max.x}, {portal1Bounds.max.x}");
        //Debug.Log($"playerBounds: min, max y {playerBounds.min.y}, {playerBounds.max.y}, playerBounds: min, max x {playerBounds.max.x}, {playerBounds.max.x}");

        if (CheckPlayerBounds())
        {
            Debug.Log($"Player within portal bounds");
        }
    }

    bool CheckPlayerBounds()
    {
        Bounds portal1Bounds = obj1.GetComponent<Renderer>().bounds;
        Bounds playerBounds = playerCollider.bounds;

        if (portal1Bounds.min.y > playerBounds.min.y || portal1Bounds.max.y < playerBounds.max.y)
        {
            return false;
        }

        return (portal1Bounds.min.x > playerBounds.min.x || portal1Bounds.max.x < playerBounds.max.x) ? false : true;
    }
}
