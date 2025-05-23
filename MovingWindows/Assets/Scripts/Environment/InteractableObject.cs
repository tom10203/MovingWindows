using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour
{
    protected bool canInteract = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            canInteract = false;
        }
    }
}
