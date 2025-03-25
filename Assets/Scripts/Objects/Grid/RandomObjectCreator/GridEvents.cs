using System;
using UnityEngine;

public static class GridEvents
{
    // Events
    public static event Action OnGridChanged;
    public static event Action OnFallingComplete;
    public static event Action OnFillingComplete;
    public static event Action<bool> OnProcessingStateChanged;

    // Current processing state
    private static bool isProcessing = false;
    public static bool IsProcessing => isProcessing;

    // Trigger methods
    public static void TriggerGridChanged()
    {
        SetProcessing(true);
        OnGridChanged?.Invoke();
    }

    public static void TriggerFallingComplete()
    {
        OnFallingComplete?.Invoke();
    }

    public static void TriggerFillingComplete()
    {
        SetProcessing(false);
        OnFillingComplete?.Invoke();
    }

    private static void SetProcessing(bool processing)
    {
        if (isProcessing != processing)
        {
            isProcessing = processing;
            OnProcessingStateChanged?.Invoke(isProcessing);
        }
    }
}