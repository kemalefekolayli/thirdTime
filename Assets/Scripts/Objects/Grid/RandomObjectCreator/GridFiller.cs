using UnityEngine;

public class GridFiller : MonoBehaviour
{
    [SerializeField] private CubeFallingAnimator fallingAnimator;
    private bool isProcessing = false;

    public bool IsProcessing => isProcessing || (fallingAnimator != null && fallingAnimator.IsAnimating);

    private void Start()
    {
        // Find the animator if not assigned
        if (fallingAnimator == null)
            fallingAnimator = FindFirstObjectByType<CubeFallingAnimator>();

        // Subscribe to falling complete event
        GridEvents.OnFallingComplete += HandleFallingComplete;
    }

    private void OnDestroy()
    {
        GridEvents.OnFallingComplete -= HandleFallingComplete;
    }

    private void HandleFallingComplete()
    {
        FillEmptySpaces();
    }

    public void FillEmptySpaces()
    {
        if (isProcessing)
            return;

        isProcessing = true;

        // Use the animator to fill cells with falling animation
        if (fallingAnimator != null)
        {
            fallingAnimator.FillEmptyCellsWithAnimation();
        }
        else
        {
            // If animator not available, just signal completion
            isProcessing = false;
            GridEvents.TriggerFillingComplete();
        }
    }
}