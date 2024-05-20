using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioSource source;

    private GridData floatationData, buildingsData;

    [SerializeField]
    private BuildPreviewSystem preview;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private GameManager gameManager;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    IPlacementState buildingState;

    [SerializeField]
    SoundFeedback soundFeedback;

    private void Start()
    {
        StopPlacement();
        gridVisualization.SetActive(false);
        floatationData = new GridData();
        buildingsData = new GridData();
    }

    private void Update()
    {
        if (buildingState == null) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUi()) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    public void StartPlacement(int Id)
    {
        StopPlacement();
        soundFeedback.PlaySound(SoundType.Click);
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(Id, grid, preview, database, floatationData, buildingsData, objectPlacer, soundFeedback, gameManager);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    private void StopPlacement()
    {
        if (buildingState == null) return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    public void StartDemolish()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new DemolishState(grid, preview, floatationData, buildingsData, objectPlacer, soundFeedback);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
}
