using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraScript : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Space]
    public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed; //seconds taken to reach max speed;
    public float zoomSpeed;

    [Header("Movement Limits")]
    [Space]
    public float minimumZoom;
    public float maximumZoom;

    private float panSpeed;
    private Vector3 panMovement;
    private float panIncrease = 0.0f;



    void Update()
    {
        panMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            panMovement += (Camera.main.orthographicSize / 10f) * Vector3.up * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            panMovement += (Camera.main.orthographicSize / 10f) * Vector3.down * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            panMovement += (Camera.main.orthographicSize / 10f) * Vector3.left * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            panMovement += (Camera.main.orthographicSize / 10f) * Vector3.right * panSpeed * Time.deltaTime;
        }

        transform.Translate(panMovement, Space.World);

        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q))
        {
            panIncrease += Time.deltaTime / secToMaxSpeed;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
        }

        else
        {
            panIncrease = 0;
            panSpeed = minPanSpeed;
        }

        Camera.main.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minimumZoom, maximumZoom);
    }
}
