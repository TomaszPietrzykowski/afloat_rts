using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayerMask;

    public event Action OnClicked, OnRotate, OnRotateBack, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnExit?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRotate?.Invoke();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            OnRotate?.Invoke();
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            OnRotateBack?.Invoke();
        }
    }

    public bool IsPointerOverUi() => EventSystem.current.IsPointerOverGameObject();
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
