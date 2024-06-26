using UnityEngine;
using static BuildPreviewSystem;

public class DemolishState : IPlacementState
{
    private int gameObjectIndex = -1;
    Grid grid;
    BuildPreviewSystem buildPreviewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback;

    public DemolishState(Grid grid,
                         BuildPreviewSystem buildPreviewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer,
                         SoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.buildPreviewSystem = buildPreviewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        buildPreviewSystem.StartShowingDemolishPreview();
    }

    public void EndState()
    {
        buildPreviewSystem.StopShowingDemolishPreview();
    }

    public void OnAction(Vector3Int gridPosition, bool isInitial, PreviewOrientation orientation)
    {
        GridData selectedData = null;
        if (furnitureData.CanPlaceBuildingAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        else if (floorData.CanPlaceFloatationAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if (selectedData == null)
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
            {
                soundFeedback.PlaySound(SoundType.wrongPlacement);
                return;
            }
            soundFeedback.PlaySound(SoundType.Remove);
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);

            Vector3 cellPosition = grid.CellToWorld(gridPosition);
            buildPreviewSystem.UpdatePosition(cellPosition, IsSelectionValid(gridPosition));
        }
    }

    private bool IsSelectionValid(Vector3Int gridPosition)
    {
        return !(furnitureData.CanPlaceBuildingAt(gridPosition, Vector2Int.one) && floorData.CanPlaceFloatationAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition, PreviewOrientation orientation)
    {
        bool validity = IsSelectionValid(gridPosition);
        buildPreviewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
    // refactor:
    public void RefreshValidation(Vector3Int gridPosition, PreviewOrientation orientation)
    {
        return;
    }
}
