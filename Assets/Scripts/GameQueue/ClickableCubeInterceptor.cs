using UnityEngine;

public class ClickableCubeInterceptor : MonoBehaviour
{
    private CubeObject cubeObject;
    private GameProcessAdapter processAdapter;

    void Start()
    {
        cubeObject = GetComponent<CubeObject>();
        processAdapter = FindFirstObjectByType<GameProcessAdapter>();

        // Disable the original ClickableCube component
        ClickableCube original = GetComponent<ClickableCube>();
        if (original != null)
        {
            original.enabled = false;
        }
    }

    void OnMouseDown()
    {
        if (cubeObject != null && processAdapter != null && !GridEvents.IsProcessing)
        {
            processAdapter.HandleCubeClick(cubeObject);
        }
    }
}

public class ClickableRocketInterceptor : MonoBehaviour
{
    private RocketObject rocketObject;
    private GameProcessAdapter processAdapter;

    void Start()
    {
        rocketObject = GetComponent<RocketObject>();
        processAdapter = FindFirstObjectByType<GameProcessAdapter>();

        // Disable the original ClickableRocket component
        ClickableRocket original = GetComponent<ClickableRocket>();
        if (original != null)
        {
            original.enabled = false;
        }
    }

    void OnMouseDown()
    {
        if (rocketObject != null && processAdapter != null && !GridEvents.IsProcessing)
        {
            processAdapter.HandleRocketClick(rocketObject);
        }
    }
}