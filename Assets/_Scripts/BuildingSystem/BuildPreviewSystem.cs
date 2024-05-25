using UnityEngine;

public class BuildPreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;
    private Vector2Int previewSize;
    public PreviewOrientation previewOrientation = PreviewOrientation.North;

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
        cellIndicator.transform.Rotate(Vector3.up, rotationAngle, Space.Self);

        var correctionOffset = GetCorrectionOffset(clockwise);
        previewObject.transform.position += correctionOffset;
        cellIndicator.transform.position += correctionOffset;
        var updatedSize = new Vector2Int(previewSize.y, previewSize.x);
        previewSize = updatedSize;
    }

    private Vector3 GetCorrectionOffset(RotationDirection clockwise)
    {
        Vector3 offset = Vector3.zero;

        switch (previewOrientation)
        {
            case PreviewOrientation.North:
                Debug.Log($"North: x: {previewSize.x}, y: {previewSize.y}");
                previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.East : PreviewOrientation.West;
                offset = clockwise == RotationDirection.Clockwise ? new Vector3(0, 0, previewSize.x) : new Vector3(previewSize.y, 0, 0);
                break;
            case PreviewOrientation.East:
                Debug.Log($"East: x: {previewSize.x}, y: {previewSize.y}");
                previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.South : PreviewOrientation.North;
                offset = clockwise == RotationDirection.Clockwise ? new Vector3(previewSize.y, 0, -1 * Mathf.Abs(previewSize.x - previewSize.y)) : new Vector3(0, 0, -previewSize.y);
                break;
            case PreviewOrientation.South:
                Debug.Log($"South: x: {previewSize.x}, y: {previewSize.y}");
                previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.West : PreviewOrientation.East;
                offset = clockwise == RotationDirection.Clockwise ? new Vector3(-1 * Mathf.Abs(previewSize.x - previewSize.y), 0, -previewSize.y) : new Vector3(-previewSize.x, 0, Mathf.Abs(previewSize.x - previewSize.y));
                break;
            case PreviewOrientation.West:
                Debug.Log($"West: x: {previewSize.x}, y: {previewSize.y}");
                previewOrientation = clockwise == RotationDirection.Clockwise ? PreviewOrientation.North : PreviewOrientation.South;
                offset = clockwise == RotationDirection.Clockwise ? new Vector3(-previewSize.x, 0, 0) : new Vector3(Mathf.Abs(previewSize.x - previewSize.y), 0, previewSize.x);
                break;
        }
        return offset;
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
        ResetCellIndicator();
        cellIndicator.SetActive(false);
        if (previewObject != null) Destroy(previewObject);
        previewOrientation = PreviewOrientation.North;
    }

    private void ResetCellIndicator()
    {
        float angle = 0f;
        switch (previewOrientation)
        {
            case PreviewOrientation.North:
                break;
            case PreviewOrientation.East:
                angle = 270;
                break;
            case PreviewOrientation.South:
                angle = 180;
                break;
            case PreviewOrientation.West:
                angle = 90;
                break;
        }
        cellIndicator.transform.Rotate(Vector3.up, angle, Space.Self);
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
        //cellIndicator.transform.position = position;
        cellIndicator.transform.position = position + GetMoveOffset();
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

    public enum PreviewOrientation
    {
        North,
        East,
        South,
        West
    }
}
