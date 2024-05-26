using UnityEngine;
using static BuildPreviewSystem;

public interface IPlacementState
{
    void EndState();
    void OnAction(Vector3Int gridPosition, bool isInitial, PreviewOrientation orientation);
    void UpdateState(Vector3Int gridPosition, PreviewOrientation orientation);
    void RefreshValidation(Vector3Int gridPosition, PreviewOrientation orientation);
}