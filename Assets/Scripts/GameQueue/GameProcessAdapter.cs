using UnityEngine;

public class GameProcessAdapter : MonoBehaviour
{
    private CubeInputHandler cubeInputHandler;
    private RocketInputHandler rocketInputHandler;
    private CubeFallingHandler fallingHandler;
    private GridFiller gridFiller;

    void Start()
    {
        // Find all necessary handlers
        cubeInputHandler = FindFirstObjectByType<CubeInputHandler>();
        rocketInputHandler = FindFirstObjectByType<RocketInputHandler>();
        fallingHandler = FindFirstObjectByType<CubeFallingHandler>();
        gridFiller = FindFirstObjectByType<GridFiller>();

        // Subscribe to grid events
        GridEvents.OnGridChanged += HandleGridChanged;
        GridEvents.OnFallingComplete += HandleFallingComplete;
    }

    private void OnDestroy()
    {
        GridEvents.OnGridChanged -= HandleGridChanged;
        GridEvents.OnFallingComplete -= HandleFallingComplete;
    }

    // Hook into input handlers
    public void HandleCubeClick(CubeObject cube)
    {
        if (cubeInputHandler != null)
        {
            GameProcessQueue.Instance.EnqueueProcess(
                new GameProcessQueue.CubeMatchProcess(cube, cubeInputHandler));
        }
    }

    public void HandleRocketClick(RocketObject rocket)
    {
        if (rocketInputHandler != null)
        {
            GameProcessQueue.Instance.EnqueueProcess(
                new GameProcessQueue.RocketMatchProcess(rocket, rocketInputHandler));
        }
    }

    // Event handlers to queue next process stages
    private void HandleGridChanged()
    {
        if (fallingHandler != null)
        {
            GameProcessQueue.Instance.EnqueueProcess(
                new GameProcessQueue.FallingProcess(fallingHandler));
        }
    }

    private void HandleFallingComplete()
    {
        if (gridFiller != null)
        {
            GameProcessQueue.Instance.EnqueueProcess(
                new GameProcessQueue.FillingProcess(gridFiller));
        }
    }
}