using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ClickableRocket : MonoBehaviour
{
    private RocketObject rocketObject;
    private RocketInputHandler inputHandler;

    void Start()
    {
        rocketObject = GetComponent<RocketObject>();
        inputHandler = Object.FindFirstObjectByType<RocketInputHandler>();
    }

    void OnMouseDown()
    {
        Debug.Log("Rocket clicked: " + gameObject.name);
        if (rocketObject != null && inputHandler != null)
        {
            inputHandler.OnRocketClicked(rocketObject);
        }
        else
        {
            Debug.LogError("Missing references: rocketObject=" + (rocketObject != null) + ", inputHandler=" + (inputHandler != null));
        }
    }
}