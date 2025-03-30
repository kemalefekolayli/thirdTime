using UnityEngine;
using System.Collections.Generic;

public class BlastNotifier : MonoBehaviour
{
    private static BlastNotifier _instance;
    public static BlastNotifier Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<BlastNotifier>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("BlastNotifier");
                    _instance = obj.AddComponent<BlastNotifier>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private int currentBlastId = 0;
    private List<IBlastObserver> observers = new List<IBlastObserver>();

    public void RegisterObserver(IBlastObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void UnregisterObserver(IBlastObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyBlast(List<Vector2Int> blastGroup)
    {
        currentBlastId++; // Generate a new blast ID
        foreach (IBlastObserver observer in observers)
        {
            observer.OnBlastOccurred(blastGroup, currentBlastId);
        }
    }
}