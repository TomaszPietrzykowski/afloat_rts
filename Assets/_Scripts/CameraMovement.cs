using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraTrasform;

    public float movementTime;

    public PlacementSystem placementSystem;

    private float movementSpeed; // 0-100
    private float movementConstantFactor = 0.001f;
    private float zoomConstantFactor = 0.03f;
    private float zoomWheelConstantFactor = 0.3f;

    public float normalSpeed;
    public float fastSpeed;
    public float rotationAmount;
    public Vector3 zoomAmount;
    public float panBorderThickness = 10f;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public Vector3 originalZoom;
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTrasform.localPosition;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalZoom = cameraTrasform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }
    private bool resetOngoing = false;
    private void ResetCamera() // TODO: fix
    {
        if (resetOngoing) return;
        resetOngoing = true;
        //transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * movementTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * movementTime);
        //cameraTrasform.localPosition = Vector3.Lerp(cameraTrasform.localPosition, originalZoom, Time.deltaTime * movementTime);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        cameraTrasform.localPosition = originalZoom;
        resetOngoing = false;
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            newPosition += transform.forward * movementSpeed * movementConstantFactor;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= panBorderThickness)
        {
            newPosition += transform.forward * -movementSpeed * movementConstantFactor;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= panBorderThickness)
        {
            newPosition += transform.right * -movementSpeed * movementConstantFactor;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            newPosition += transform.right * movementSpeed * movementConstantFactor;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += (zoomAmount * zoomConstantFactor);
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= (zoomAmount * zoomConstantFactor);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !placementSystem.IsActiveBuildingState())
        {
            newZoom += (zoomAmount * zoomWheelConstantFactor);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && !placementSystem.IsActiveBuildingState())
        {
            newZoom -= (zoomAmount * zoomWheelConstantFactor);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetCamera();
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTrasform.localPosition = Vector3.Lerp(cameraTrasform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
