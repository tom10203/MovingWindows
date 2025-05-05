using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCameraMovement : MonoBehaviour
{
    // This scripts is attatched to the camera that follows the player
    // It is responsible for moving the other cameras that render the background at a slower pace to give perspective movement feel

    [SerializeField] private Camera[] perspectiveCameras;
    [SerializeField] private Camera[] staticCameras;

    [SerializeField] private float cameraMoveAmount1 = 0.8f;
    [SerializeField] private float cameraMoveDifference = 0.2f;
    [SerializeField] private float cameraMoveAmount2 = 0.6f;

    [SerializeField] private float moveSpeed = 10f;
    private Vector2 oldPosition = Vector2.zero;

    private float minSize; // This size is set when there are no portals in the game
    [SerializeField] private float maxSize;

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        minSize = cam.orthographicSize;
        maxSize = minSize;
    }
    void LateUpdate()
    {
        Vector2 movement = transform.position - (Vector3)oldPosition;
        MoveCameras(movement);
        oldPosition = transform.position;

        //cam.orthographicSize = maxSize;

    }

    private void MoveCameras(Vector2 movement)
    {
        for (int i = 0; i < perspectiveCameras.Length; i++)
        {
            Camera camera = perspectiveCameras[i];
            float moveAmount = cameraMoveAmount1 - cameraMoveDifference * i;
            camera.transform.Translate(moveAmount * movement);
            //SetSize(camera, maxSize);   

        }
        

        foreach (var camera in staticCameras)
        {
            camera.transform.position = transform.position;
            //SetSize(camera, maxSize);
        }
    }

    private void SetSize(Camera cam,  float size)
    {
        cam.orthographicSize = size;
    }

    
}
