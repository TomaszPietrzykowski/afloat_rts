using UnityEngine;

public interface IPlacementState
{
    void EndState();
    void OnAction(Vector3Int gridPosition, bool isInitial);
    void UpdateState(Vector3Int gridPosition);
}