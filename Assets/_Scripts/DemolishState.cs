using UnityEngine;

public class DemolishState : IPlacementState
{
    private int gameObjectIndex = -1;
    Grid grid;
    BuildPreviewSystem buildPreviewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public DemolishState(Grid grid,
                         BuildPreviewSystem buildPreviewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.buildPreviewSystem = buildPreviewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        buildPreviewSystem.StartShowingDemolishPreview();
    }

    public void EndState()
    {
        buildPreviewSystem.StartShowingDemolishPreview();
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
            // sound indication
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1) return;

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
