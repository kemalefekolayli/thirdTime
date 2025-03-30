using UnityEngine;

public class DefeatScreen : MonoBehaviour
{

    [SerializeField] private GameObject DefeatScreenObject;


    // Static instance for global access
    private static DefeatScreen instance;

    private void Awake()
    {
        instance = this;

        // Hide the victory screen initially
        if (DefeatScreenObject != null)
        {
            DefeatScreenObject.SetActive(false);
        }
    }

    // Static method to show the victory screen
    public static void Show()
    {
        if (instance != null && instance.DefeatScreenObject != null)
        {
            // Activate the GameObject
            instance.DefeatScreenObject.SetActive(true);

            // Make sure it's in front by moving it slightly forward in Z
            instance.DefeatScreenObject.transform.position = new Vector3(
                instance.DefeatScreenObject.transform.position.x,
                instance.DefeatScreenObject.transform.position.y,
                instance.DefeatScreenObject.transform.position.y
                );

            Debug.LogError("Victory screen shown!");
        }
        else
        {
            Debug.LogError("Victory screen reference is missing!");
        }
    }
}