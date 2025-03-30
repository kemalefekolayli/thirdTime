using UnityEngine;


public class GameInitializer : MonoBehaviour
{
    // Prefabs to instantiate if they don't exist in the scene
    [Header("Core Systems")]
    [SerializeField] private GameObject levelLoaderPrefab;
    [SerializeField] private GameObject levelProgressManagerPrefab;
    [SerializeField] private GameObject sceneControllerPrefab;
    [SerializeField] private GameObject celebrationManagerPrefab;

    private void Awake()
    {
        // Make sure the LevelLoader system exists
        if (FindFirstObjectByType<LevelLoader>() == null && levelLoaderPrefab != null)
        {
            Instantiate(levelLoaderPrefab);
        }

        // Make sure the LevelProgressManager system exists
        if (LevelProgressManager.Instance == null && levelProgressManagerPrefab != null)
        {
            Instantiate(levelProgressManagerPrefab);
        }

        // Make sure the SceneController system exists
        if (SceneController.Instance == null && sceneControllerPrefab != null)
        {
            Instantiate(sceneControllerPrefab);
        }

        // Make sure the CelebrationManager system exists
        if (CelebrationManager.Instance == null && celebrationManagerPrefab != null)
        {
            Instantiate(celebrationManagerPrefab);
        }
    }
}