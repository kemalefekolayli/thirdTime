using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    // Reference to the victory screen GameObject
    [SerializeField] private GameObject victoryScreenObject;


    // Static instance for global access
    private static VictoryScreen instance;

    private void Awake()
    {
        instance = this;

        // Hide the victory screen initially
        if (victoryScreenObject != null)
        {
            victoryScreenObject.SetActive(false);
        }
    }

    // Static method to show the victory screen
    public static void Show()
    {
        if (instance != null && instance.victoryScreenObject != null)
        {
            // Activate the GameObject
            instance.victoryScreenObject.SetActive(true);

            // Make sure it's in front by moving it slightly forward in Z
            instance.victoryScreenObject.transform.position = new Vector3(
                instance.victoryScreenObject.transform.position.x,
                instance.victoryScreenObject.transform.position.y,
                instance.victoryScreenObject.transform.position.y
                );

            Debug.LogError("Victory screen shown!");
        }
        else
        {
            Debug.LogError("Victory screen reference is missing!");
        }
    }
}