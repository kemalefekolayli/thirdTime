using UnityEngine;
using System.Collections.Generic;

public interface IBlastObserver
{
    void OnBlastOccurred(List<Vector2Int> blastGroup, int blastId);
}