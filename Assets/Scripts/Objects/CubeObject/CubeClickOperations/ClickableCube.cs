using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ClickableCube : MonoBehaviour
{
    private CubeObject cubeObject;
    private CubeInputHandler inputHandler;

    void Start()
    {
        cubeObject = GetComponent<CubeObject>();
        inputHandler = Object.FindFirstObjectByType<CubeInputHandler>();
    }

    void OnMouseDown()
    {
        Debug.Log("Cube clicked: " + gameObject.name);
        if (cubeObject != null && inputHandler != null)
        {
            inputHandler.OnCubeClicked(cubeObject);
        }
        else
        {
            Debug.LogError("Missing references: cubeObject=" + (cubeObject != null) + ", inputHandler=" + (inputHandler != null));
        }
    }
}