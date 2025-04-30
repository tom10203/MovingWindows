using UnityEngine;

public class PerspectiveCameraMovement : MonoBehaviour
{
    // This scripts is attatched to the camera that follows the player
    // It is responsible for moving the other cameras that render the background at a slower pace to give perspective movement feel

    [SerializeField] private Camera[] cameras;

    [SerializeField] private float cameraMoveAmount1 = 0.8f;
    [SerializeField] private float cameraMoveAmount2 = 0.6f;

    [SerializeField] private float moveSpeed = 10f;


    void Update()
    {

    }

    private void MoveCameras(Vector2 movement)
    {
        cameras[0].transform.Translate(movement * cameraMoveAmount1);
        cameras[1].transform.Translate(movement * cameraMoveAmount2);
    }
}
