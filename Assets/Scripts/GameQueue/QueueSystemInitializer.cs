using UnityEngine;

public class QueueSystemInitializer : MonoBehaviour
{
    void Start()
    {
        // Make sure GameProcessQueue exists
        GameProcessQueue queue = GameProcessQueue.Instance;

        // Add interceptors to all objects with the original click handlers
        AddInterceptorsToCubes();
        AddInterceptorsToRockets();
    }

    private void AddInterceptorsToCubes()
    {
        ClickableCube[] cubes = Object.FindObjectsByType<ClickableCube>(FindObjectsSortMode.None);
        foreach (ClickableCube cube in cubes)
        {
            if (!cube.GetComponent<ClickableCubeInterceptor>())
            {
                cube.gameObject.AddComponent<ClickableCubeInterceptor>();
            }
        }
    }

    private void AddInterceptorsToRockets()
    {
        ClickableRocket[] rockets = Object.FindObjectsByType<ClickableRocket>(FindObjectsSortMode.None);
        foreach (ClickableRocket rocket in rockets)
        {
            if (!rocket.GetComponent<ClickableRocketInterceptor>())
            {
                rocket.gameObject.AddComponent<ClickableRocketInterceptor>();
            }
        }
    }

    // Method to handle newly created objects
    public void AddInterceptorsToNewObjects()
    {
        AddInterceptorsToCubes();
        AddInterceptorsToRockets();
    }
}