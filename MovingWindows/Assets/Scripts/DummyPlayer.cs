using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerSprite;

    public Vector3 offset;
    

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
        transform.rotation = playerSprite.rotation;
    }
}
