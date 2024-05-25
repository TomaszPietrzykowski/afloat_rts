using System.Collections.Generic;
using UnityEngine;
using static BuildPreviewSystem;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position, PreviewOrientation orientation, Vector2Int size)
    {
        int x = size.x;
        int y = size.y;
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        if (orientation == PreviewOrientation.East)
        {
            newObject.transform.Rotate(Vector3.up, 90f, Space.Self);
            newObject.transform.position += new Vector3(0, 0, x);
        }
        if (orientation == PreviewOrientation.South)
        {
            newObject.transform.Rotate(Vector3.up, 180f, Space.Self);
            newObject.transform.position += new Vector3(x, 0, y);
        }
        if (orientation == PreviewOrientation.West)
        {
            newObject.transform.Rotate(Vector3.up, 270f, Space.Self);
            newObject.transform.position += new Vector3(y, 0, 0);
        }
        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null) return;

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
