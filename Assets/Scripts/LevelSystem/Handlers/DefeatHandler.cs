using UnityEngine;


public class DefeatHandler : MonoBehaviour
{
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject tryAgainButton;

    private void Start()
    {
        // Get button components if not assigned in inspector
        if (closeButton == null)
        {
            closeButton = transform.Find("CloseButton")?.gameObject;
        }

        if (tryAgainButton == null)
        {
            tryAgainButton = transform.Find("TryAgainButton")?.gameObject;
        }

        // Set up button event handlers
        if (closeButton != null)
        {
            SimpleClickHandler clickHandler = closeButton.GetComponent<SimpleClickHandler>();
            if (clickHandler == null)
            {
                clickHandler = closeButton.AddComponent<SimpleClickHandler>();
            }
            clickHandler.actionType = SimpleClickHandler.ClickAction.GoToMainScene;
        }

        if (tryAgainButton != null)
        {
            SimpleClickHandler clickHandler = tryAgainButton.GetComponent<SimpleClickHandler>();
            if (clickHandler == null)
            {
                clickHandler = tryAgainButton.AddComponent<SimpleClickHandler>();
            }
            clickHandler.actionType = SimpleClickHandler.ClickAction.RestartLevel;
        }
    }
}