using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private int maxLevel = 10; // Total number of levels

    private int currentLevel = 1;


    private void Start()
    {
        Debug.Log("[LevelButton] Start method called");

        // Check if we have the required Collider2D
        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("[LevelButton] Missing Collider2D component! Adding BoxCollider2D");
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1, 1); // Default size
        }
        else
        {
            Debug.Log("[LevelButton] Found Collider2D: " + collider.GetType().Name);
        }

        LoadCurrentLevel();
        UpdateButtonText();

        Debug.Log("[LevelButton] Initialization complete");
    }

    private void OnEnable()
    {
        Debug.Log("[LevelButton] OnEnable called");
    }

    private void LoadCurrentLevel()
    {
        Debug.Log("[LevelButton] Loading current level");

        // Try LevelProgressManager first
        if (LevelProgressManager.Instance != null)
        {
            currentLevel = LevelProgressManager.Instance.CurrentLevel;
            Debug.Log($"[LevelButton] Level from LevelProgressManager: {currentLevel}");
        }
        else
        {
            // Fallback to PlayerPrefs
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            Debug.Log($"[LevelButton] Level from PlayerPrefs: {currentLevel}");
        }
    }

    private void UpdateButtonText()
    {
        Debug.Log("[LevelButton] Updating button text");

        if (levelText != null)
        {
            if (currentLevel > maxLevel)
            {
                levelText.text = "Finished";
            }
            else
            {
                levelText.text = $"Level {currentLevel}";
            }
            Debug.Log($"[LevelButton] Text set to: {levelText.text}");
        }
        else
        {
            Debug.LogWarning("[LevelButton] levelText is null, trying to find in children");

            // Try to find TextMeshProUGUI component in children if not assigned
            levelText = GetComponentInChildren<TextMeshProUGUI>();

            if (levelText != null)
            {
                Debug.Log("[LevelButton] Found TextMeshProUGUI in children");
                UpdateButtonText(); // Try again with found text component
            }
            else
            {
                Debug.LogError("[LevelButton] TextMeshProUGUI component not found anywhere!");
            }
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("[LevelButton] OnMouseDown called - Button clicked!");
        LoadLevel();
    }

    public void OnPointerClick()
    {
        Debug.Log("[LevelButton] OnPointerClick called from external source");
        LoadLevel();
    }

    private void LoadLevel()
    {
        Debug.Log($"[LevelButton] LoadLevel called, current level: {currentLevel}, max level: {maxLevel}");

        // If all levels are completed, do nothing
        if (currentLevel > maxLevel)
        {
            Debug.Log("[LevelButton] All levels completed, not loading");
            return;
        }

        // Try SceneController first
        if (SceneController.Instance != null)
        {
            Debug.Log("[LevelButton] Using SceneController to load level scene");
            SceneController.Instance.LoadLevelScene();
        }
        else
        {
            // Fall back to direct scene loading
            Debug.Log("[LevelButton] SceneController not found, using SceneManager directly");
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
                Debug.Log("[LevelButton] LoadScene called for LevelScene");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[LevelButton] Error loading scene: {ex.Message}");
            }
        }
    }

    void Update()
    {
        // Debug hover state
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePosition);

            if (hit != null && hit.gameObject == gameObject)
            {
                Debug.Log("[LevelButton] Detected click through Physics2D on this button");
            }
        }
    }

    /// <summary>
    /// Public method to update the level from outside
    /// </summary>
    public void UpdateLevel(int newLevel)
    {
        Debug.Log($"[LevelButton] UpdateLevel called with level: {newLevel}");
        currentLevel = newLevel;
        UpdateButtonText();
    }
}