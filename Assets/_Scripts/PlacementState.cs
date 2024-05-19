using System;
using UnityEngine;

public class PlacementState : IPlacementState
{
    private int selectedObjectIndex = -1;
    int Id;
    Grid grid;
    BuildPreviewSystem buildPreviewSystem;
    ObjectDatabaseSO database;
    GridData raftData;
    GridData buildingData;
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback;

    public PlacementState(int id,
                          Grid grid,
                          BuildPreviewSystem buildPreviewSystem,
                          ObjectDatabaseSO database,
                          GridData raftData,
                          GridData buildingData,
                          ObjectPlacer objectPlacer,
                          SoundFeedback soundFeedback)
    {
        Id = id;
        this.grid = grid;
        this.buildPreviewSystem = buildPreviewSystem;
        this.database = database;
        this.raftData = raftData;
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.Id == Id);
        if (selectedObjectIndex > -1)
        {
            buildPreviewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new Exception($"Could not find object with Id {Id}");
        }
    }

    public void EndState()
    {
        buildPreviewSystem.StopShowingPlacementPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            return;
        }

        soundFeedback.PlaySound(SoundType.Place);
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        GridData selectedData = database.objectsData[selectedObjectIndex].Id == 0 ? raftData : buildingData;
        selectedData.AddObjectAt(gridPosition,
                                 database.objectsData[selectedObjectIndex].Size,
                                 database.objectsData[selectedObjectIndex].Id,
                                 index);
        buildPreviewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].Id == 0 ? raftData : buildingData;
        if (database.objectsData[selectedObjectIndex].Id == 0)
        {
            return raftData.CanPlaceFloatationAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
        }
        else if (buildingData.CanPlaceBuildingAt(gridPosition, database.objectsData[selectedObjectIndex].Size)
            && raftData.IsRaftAvailaible(gridPosition, database.objectsData[selectedObjectIndex].Size))
        {
            return true;
        }
        return false;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        buildPreviewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
