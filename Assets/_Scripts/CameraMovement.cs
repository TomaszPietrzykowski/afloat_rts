using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraTransform;

    public float movementTime;

    public PlacementSystem placementSystem;

    private float movementSpeed; // 0-100
    private float movementConstantFactor = 0.001f;
    private float zoomConstantFactor = 0.03f;
    private float zoomWheelConstantFactor = 0.3f;

    public Vector2 panLimit;

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

    public GameManager gameManager;
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalZoom = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMovementInput();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (resetOngoing) return;
            StartCoroutine(ResetCamera());
        }
    }
    private bool resetOngoing = false;
    private IEnumerator ResetCamera()
    {
        resetOngoing = true;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 startZoom = cameraTransform.localPosition;

        float elapsedTime = 0f;
        float resetTime = 1f;

        while (elapsedTime < resetTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / resetTime);

            transform.position = Vector3.Lerp(startPosition, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, originalRotation, t);
            cameraTransform.localPosition = Vector3.Lerp(startZoom, originalZoom, t);

            yield return null;
        }

        newPosition = originalPosition;
        newRotation = originalRotation;
        newZoom = originalZoom;

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
        if (Input.GetKey(KeyCode.R) && !gameManager.IsOngoingBuildingPlacement)
        {
            newZoom += (zoomAmount * zoomConstantFactor);
        }
        if (Input.GetKey(KeyCode.F) && !gameManager.IsOngoingBuildingPlacement)
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

        newPosition.x = Mathf.Clamp(newPosition.x, -panLimit.x, panLimit.x);
        newPosition.z = Mathf.Clamp(newPosition.z, -panLimit.y, panLimit.y);

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
