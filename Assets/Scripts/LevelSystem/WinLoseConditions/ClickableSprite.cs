using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SimpleClickHandler : MonoBehaviour
{
    // Define what action happens when the sprite is clicked
    public enum ClickAction
    {
        GoToMainScene,
        RestartLevel,
        Custom
    }

    [SerializeField] public ClickAction actionType = ClickAction.GoToMainScene;

    private void OnMouseDown()
    {
        switch (actionType)
        {
            case ClickAction.GoToMainScene:
                LoadMainScene();
                break;

            case ClickAction.RestartLevel:
                RestartLevel();
                break;

            case ClickAction.Custom:
                // Custom action can be implemented in derived classes
                break;
        }
    }

    private void LoadMainScene()
    {
        Debug.Log("Loading main scene");
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainScene();
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    private void RestartLevel()
    {
        Debug.Log("Restarting level");
        if (SceneController.Instance != null)
        {
            SceneController.Instance.RestartLevel();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}