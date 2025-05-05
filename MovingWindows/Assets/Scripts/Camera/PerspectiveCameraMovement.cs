using UnityEngine;

public class PerspectiveCameraMovement : MonoBehaviour
{
    // This scripts is attatched to the camera that follows the player
    // It is responsible for moving the other cameras that render the background at a slower pace to give perspective movement feel

    [SerializeField] private Camera[] perspectiveCameras;
    [SerializeField] private Camera[] staticCameras;

    [SerializeField] private float cameraMoveAmount1 = 0.8f;
    [SerializeField] private float cameraMoveAmount2 = 0.6f;

    [SerializeField] private float moveSpeed = 10f;
    private Vector2 oldPosition = Vector2.zero;


    void LateUpdate()
    {
        Vector2 movement = transform.position - (Vector3)oldPosition;
        MoveCameras(movement);
        oldPosition = transform.position;

    }

    private void MoveCameras(Vector2 movement)
    {
        perspectiveCameras[0].transform.Translate(movement * cameraMoveAmount1);
        perspectiveCameras[1].transform.Translate(movement * cameraMoveAmount2);

        foreach (var camera in staticCameras)
        {
            camera.transform.position = transform.position;
        }
    }
}
