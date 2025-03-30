using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    // Reference to the victory screen GameObject
    [SerializeField] private GameObject victoryScreenObject;
public static GameObject victoryScreenPrefab;
private static VictoryScreen instance;

private void Awake()
{
    instance = this;
    gameObject.SetActive(false);
}

public static void Show()
{
    // Load prefab if not already loaded
    if (victoryScreenPrefab == null)
        victoryScreenPrefab = Resources.Load<GameObject>("Prefabs/VictoryScreen");

    if (victoryScreenPrefab != null)
    {
        // Instantiate the screen
        GameObject screen = Instantiate(victoryScreenPrefab);

        // Position in center of screen in world space
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            screen.GetComponent<RectTransform>().position = canvasRect.position;
        }

        // Make sure it's active
        screen.SetActive(true);

        Debug.Log("Victory screen shown!");
    }
    else
    {
        Debug.LogError("Victory screen prefab not found!");
    }
}
}