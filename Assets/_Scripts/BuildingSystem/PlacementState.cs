using System;
using System.Collections.Generic;
using UnityEngine;
using static BuildPreviewSystem;

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
    GameManager gameManager;

    public PlacementState(int id,
                          Grid grid,
                          BuildPreviewSystem buildPreviewSystem,
                          ObjectDatabaseSO database,
                          GridData raftData,
                          GridData buildingData,
                          ObjectPlacer objectPlacer,
                          SoundFeedback soundFeedback,
                          GameManager gameManager)
    {
        Id = id;
        this.grid = grid;
        this.buildPreviewSystem = buildPreviewSystem;
        this.database = database;
        this.raftData = raftData;
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        this.gameManager = gameManager;

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

    public void OnAction(Vector3Int gridPosition, bool isInitial, PreviewOrientation orientation)
    {
        if (isInitial is not true)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
            if (placementValidity == false)
            {
                soundFeedback.PlaySound(SoundType.wrongPlacement);
                return;
            }
            PayForBuilding(GetCostDictionary(database.objectsData[selectedObjectIndex]));
            soundFeedback.PlaySound(database.objectsData[selectedObjectIndex].Id == 0 ? SoundType.PlaceRaft : SoundType.PlaceBuilding);
        }

        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), orientation, database.objectsData[selectedObjectIndex].Size);

        GridData selectedData = database.objectsData[selectedObjectIndex].Id == 0 ? raftData : buildingData;
        selectedData.AddObjectAt(gridPosition,
                                 database.objectsData[selectedObjectIndex].Size,
                                 database.objectsData[selectedObjectIndex].Id,
                                 index,
                                 GetCostDictionary(database.objectsData[selectedObjectIndex]));
        buildPreviewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        return true;
        //GridData selectedData = database.objectsData[selectedObjectIndex].Id == 0 ? raftData : buildingData;
        //if (database.objectsData[selectedObjectIndex].Id == 0)
        //{
        //    if (raftData.CanPlaceFloatationAt(gridPosition, database.objectsData[selectedObjectIndex].Size))
        //    {
        //        return CanAffordBuilding(GetCostDictionary(database.objectsData[selectedObjectIndex]));
        //    }
        //}
        //else if (buildingData.CanPlaceBuildingAt(gridPosition, database.objectsData[selectedObjectIndex].Size)
        //    && raftData.IsRaftAvailaible(gridPosition, database.objectsData[selectedObjectIndex].Size))
        //{
        //    return CanAffordBuilding(GetCostDictionary(database.objectsData[selectedObjectIndex]));
        //}
        //return false;
    }

    private bool CanAffordBuilding(Dictionary<GameResource, int> cost)
    {
        if (cost.ContainsKey(GameResource.Plastic))
        {
            cost.TryGetValue(GameResource.Plastic, out int value);
            if (value > gameManager.plastic) return false;
        }
        if (cost.ContainsKey(GameResource.DriftWood))
        {
            cost.TryGetValue(GameResource.DriftWood, out int value);
            if (value > gameManager.driftWood) return false;
        }
        return true;
    }

    private void PayForBuilding(Dictionary<GameResource, int> cost)
    {
        if (cost.ContainsKey(GameResource.Plastic))
        {
            cost.TryGetValue(GameResource.Plastic, out int value);
            gameManager.plastic -= value;
        }
        if (cost.ContainsKey(GameResource.DriftWood))
        {
            cost.TryGetValue(GameResource.DriftWood, out int value);
            gameManager.driftWood -= value;
        }
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        buildPreviewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }

    private Dictionary<GameResource, int> GetCostDictionary(ObjectData objectData)
    {
        Dictionary<GameResource, int> newCostDist = new();
        if (objectData.CostPlastic > 0) newCostDist.Add(GameResource.Plastic, objectData.CostPlastic);
        if (objectData.CostWood > 0) newCostDist.Add(GameResource.DriftWood, objectData.CostWood);

        return newCostDist;
    }
}
