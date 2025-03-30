using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleLevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button buttonComponent;

    void Start()
    {
        Debug.Log("SimpleLevelButton started");

        // Get components if not assigned
        if (buttonComponent == null)
            buttonComponent = GetComponent<Button>();

        if (levelText == null)
            levelText = GetComponentInChildren<TextMeshProUGUI>();

        // Add click listener
        buttonComponent.onClick.AddListener(OnButtonClick);

        // Set initial text
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        levelText.text = $"Level {level}";

        Debug.Log($"Button initialized with level {level}");
    }

    void OnButtonClick()
    {
        Debug.Log("Button clicked, loading level scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
    }
}