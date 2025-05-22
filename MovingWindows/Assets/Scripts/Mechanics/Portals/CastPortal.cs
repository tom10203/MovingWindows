using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CastPortal : InPlayScript
{
    [SerializeField] float castDst;
    [SerializeField] PlayerInput input;
    [SerializeField] LayerMask mask;
    [SerializeField] Camera cam;
    [SerializeField] GameObject spell;
    Vector3 hitPoint;
    int noOfItterations = 0;

    List<Vector3> points = new List<Vector3>();
    Vector3 hitNormal;
    public bool canCastPortal;



    public void PerformCast(Vector3 mousePos)
    {
        mousePos.z = transform.position.z;
        Vector3 direction = (mousePos - transform.position).normalized * castDst;
        points.Clear();
        Cast(transform.position, direction);
    }

    bool Cast(Vector3 raycastOrigin, Vector3 rayDirection)
    {

        raycastOrigin.z = 1f;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, rayDirection, rayDirection.magnitude, mask);

        if (!hit)
        {
            Vector3 point = raycastOrigin + rayDirection;
            point.z = 1f;
            points.Add(point);
            hitNormal = Vector3.zero;
            InstantiateSpell(false);
            return false;
        }
        else if (hit.collider.gameObject.layer == 9 && hit.collider.gameObject.tag == "Moss")
        {
            Vector3 point = hit.point;
            point.z = 1f;
            points.Add(point);
            hitNormal = AdjustHitNormal(hit.normal);
            InstantiateSpell(true);
            return true;
  
        }
        else
        {
            Vector3 point = hit.point;
            point.z = 1f;
            points.Add(point);
            Vector3 newOrigin = hit.point + hit.normal * 0.01f;
            float newDst = rayDirection.magnitude - hit.distance;
            Vector3 newDir = Vector2.Reflect(rayDirection, hit.normal);
            newDir = newDir.normalized * newDst;
            return Cast(newOrigin, newDir);
        }
    }

    Vector2 AdjustHitNormal(Vector2 normal) // Hvaing to add this as tilemap colliders use custom physics shape.
    {
        float horizontlDotProduct = Vector2.Dot(normal, Vector2.right);

        if (Mathf.Abs(horizontlDotProduct) > 0.9f)
        {
            return horizontlDotProduct > 0 ? Vector2.right : Vector2.left;
        }

        float VerticalDotProduct = Vector2.Dot(normal, Vector2.up);

        if (Mathf.Abs(VerticalDotProduct) > 0.9f)
        {
            return VerticalDotProduct > 0 ? Vector2.up : Vector2.down;
        }

        Debug.Log($"Collider normal not close to standard direction vector");
        return Vector2.zero;
    }

    void InstantiateSpell(bool castPortal)
    {
        GameObject newSpell = Instantiate(spell, transform.position, Quaternion.identity);
        SpellCast spellCast = newSpell.GetComponent<SpellCast>();
        spellCast.points = points.ToArray();
        spellCast.hitNormal = hitNormal;
        spellCast.castPortal = castPortal;

    }


}
