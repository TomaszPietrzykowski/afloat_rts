using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int id,
                            int placedObjectIndex,
                            Dictionary<GameResource, int> cost)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, id, placedObjectIndex, cost);
        foreach (var position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position))
            {
                throw new Exception($"Dictionary already contains this position: ${position}");
            }

            placedObjects[position] = data;
        }
    }
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValues = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnValues.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return returnValues;
    }

    private List<Vector3Int> CalculateAdjescentPositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValues = new();
        for (int x = -1; x <= objectSize.x; x++)
        {
            for (int y = -1; y <= objectSize.y; y++)
            {
                returnValues.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return returnValues;
    }

    public bool CanPlaceFloatationAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        List<Vector3Int> adjescentPositions = CalculateAdjescentPositions(gridPosition, objectSize);
        foreach (var position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position)) return false;
        }
        if (placedObjects.Count == 0)
        {
            return true;
        }
        else
        {
            foreach (var position in adjescentPositions)
            {
                if (placedObjects.ContainsKey(position)) return true;
            }
        }

        return false;
    }

    public bool CanPlaceBuildingAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var position in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(position)) return false;
        }

        return true;
    }

    public bool IsRaftAvailaible(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var position in positionsToOccupy)
        {
            if (!placedObjects.ContainsKey(position)) return false;
        }

        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false) return -1;

        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var position in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(position);
        }
    }
}

internal class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int Id { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public Dictionary<GameResource, int> Cost { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex, Dictionary<GameResource, int> cost)
    {
        this.occupiedPositions = occupiedPositions;
        Id = id;
        PlacedObjectIndex = placedObjectIndex;
        Cost = cost;
    }
}

public enum GameResource
{
    Plastic,
    DriftWood,
    FishRaw,
    FreshWater,
    Food
}