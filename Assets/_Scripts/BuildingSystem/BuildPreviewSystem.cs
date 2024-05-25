using UnityEngine;

public class BuildPreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;
    private Vector2Int previewSize;
    private PreviewOrientation previewOrientation = PreviewOrientation.North;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)// receive rotation?
    {
        previewObject = Instantiate(prefab);
        previewSize = size;
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }
    public void RotatePlacementPreview(RotationDirection clockwise)
    {
        float rotationAngle = clockwise == RotationDirection.Clockwise ? 90f : -90f;
        previewObject.transform.Rotate(Vector3.up, rotationAngle, Space.Self);

        var correctionOffset = GetCorrectionOffset(clockwise);
        previewObject.transform.position += correctionOffset;
        var updatedSize = new Vector2Int(previewSize.y, previewSize.x);
        previewSize = updatedSize;
    }
    private Vector3 GetCorrectionOffset(RotationDirection clockwise)
    {
        if (previewOrientation == PreviewOrientation.North)
        {
            previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.East : PreviewOrientation.West;
            return clockwise == RotationDirection.Clockwise ? new Vector3(0, 0, previewSize.x) : new Vector3(previewSize.y, 0, 0);
        }
        if (previewOrientation == PreviewOrientation.East)
        {
            previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.South : PreviewOrientation.North;
            return clockwise == RotationDirection.Clockwise ? new Vector3(previewSize.y, 0, (-1 * previewSize.x)) : new Vector3(0, 0, -1 * previewSize.y);
        }
        if (previewOrientation == PreviewOrientation.South)
        {
            previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.West : PreviewOrientation.East;
            return clockwise == RotationDirection.Clockwise ? new Vector3(-1 * previewSize.y, 0, -1 * previewSize.y) : new Vector3(-1 * previewSize.x, 0, previewSize.y);
        }
        previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.North : PreviewOrientation.South;
        return clockwise == RotationDirection.Clockwise ? new Vector3(-1 * previewSize.x, 0, 0) : new Vector3(previewSize.x, 0, previewSize.x);
    }
    private Vector3 GetMoveOffset()
    {
        if (previewOrientation == PreviewOrientation.North)
        {
            return new Vector3(0, 0, 0);
        }
        if (previewOrientation == PreviewOrientation.East)
        {
            return new Vector3(0, 0, previewSize.y);
        }
        if (previewOrientation == PreviewOrientation.South)
        {
            return new Vector3(previewSize.x, 0, previewSize.y);
        }
        return new Vector3(previewSize.x, 0, 0);
    }
    public void StopShowingPlacementPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null) Destroy(previewObject);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color color = validity ? Color.white : Color.red;
        color.a = 0.5f;
        previewMaterialInstance.color = color;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color color = validity ? Color.white : Color.red;
        color.a = 0.5f;
        cellIndicatorRenderer.material.color = color;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z) + GetMoveOffset();
    }

    internal void StartShowingDemolishPreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    internal void StopShowingDemolishPreview()
    {
        cellIndicator.SetActive(false);
    }

    private enum PreviewOrientation
    {
        North,
        East,
        South,
        West
    }
}
