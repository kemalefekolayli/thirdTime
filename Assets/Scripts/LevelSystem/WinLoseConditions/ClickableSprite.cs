using UnityEngine;

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

    [SerializeField] private ClickAction actionType = ClickAction.GoToMainScene;

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
                break;
        }
    }

    private void LoadMainScene()
    {
        Debug.Log("Loading main scene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene3");

    }

    private void RestartLevel()
    {
        Debug.Log("Restarting level");

    }
}