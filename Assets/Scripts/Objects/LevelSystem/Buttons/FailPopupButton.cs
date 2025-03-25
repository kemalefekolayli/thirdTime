using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FailPopupButton : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button tryAgainButton;

    private void Awake()
    {
        // Make sure the popup is hidden initially
        gameObject.SetActive(false);

        // Set up button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (tryAgainButton != null)
            tryAgainButton.onClick.AddListener(OnTryAgainClicked);
    }

    public void Show()
    {
        Debug.Log("FailPopup.Show() called");
        gameObject.SetActive(true);
    }

    private void OnCloseClicked()
    {
        // Return to main menu
        SceneManager.LoadScene("MainScene");
    }

    private void OnTryAgainClicked()
    {
        // Reload current level
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}