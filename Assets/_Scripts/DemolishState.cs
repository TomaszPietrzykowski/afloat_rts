using UnityEngine;

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

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
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
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = IsSelectionValid(gridPosition);
        buildPreviewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
