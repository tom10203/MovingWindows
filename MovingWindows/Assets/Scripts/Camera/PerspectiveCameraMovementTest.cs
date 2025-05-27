using UnityEngine;

public class PerspectiveCameraMovementTest : MonoBehaviour
{

    [SerializeField] private Camera[] perspectiveCameras;
    [SerializeField] private Camera[] staticCameras;
    [SerializeField] private Camera[] textureCams;

    [SerializeField] private float cameraMoveAmount1 = 0.8f;
    [SerializeField] private float cameraMoveDifference = 0.2f;
    [SerializeField] private float cameraMoveAmount2 = 0.6f;

    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] Camera textureCamMain;
    private Vector2 oldPosition = Vector2.zero;

    private float minSize; // This size is set when there are no portals in the game
    [SerializeField] private float maxSize;
    public bool isRenderCam;

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
        if (isRenderCam)
        {
            textureCamMain.transform.position = transform.position;
        }

    }

    private void MoveCameras(Vector2 movement)
    {
        for (int i = 0; i < perspectiveCameras.Length; i++)
        {
            Camera camera = perspectiveCameras[i];
            float moveAmount = cameraMoveAmount1 - cameraMoveDifference * i;
            camera.transform.Translate(moveAmount * movement);

            Vector3 offset = camera.transform.position - transform.position;

            Camera textureCam = textureCams[i];
            textureCam.transform.position = textureCamMain.transform.position + offset;
            //SetSize(camera, maxSize);   

        }

        //for (int i = 0; i < textureCams.Length; i++)
        //{
        //    Camera camera = textureCams[i];
        //    float moveAmount = cameraMoveAmount1 - cameraMoveDifference * i;
        //    camera.transform.Translate(moveAmount * movement);
        //    //SetSize(camera, maxSize);   

        //}

        foreach (var camera in staticCameras)
        {
            camera.transform.position = transform.position;
            //SetSize(camera, maxSize);
        }
    }

    private void SetSize(Camera cam, float size)
    {
        cam.orthographicSize = size;
    }



}
