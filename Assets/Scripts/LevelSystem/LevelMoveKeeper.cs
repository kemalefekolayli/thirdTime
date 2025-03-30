using UnityEngine;
using System.Collections.Generic;

public class LevelMoveKeeper : MonoBehaviour {
    public int maxMoves;
    public int currentMoves;


    void Start(){
    LevelData level3 = LevelLoader.Instance.GetLevel(1);
    if (level3 == null)
    {
                Debug.LogError("Level data is NULL.");
                return;
    }
    maxMoves = level3.move_count;
    currentMoves = level3.move_count;
    }

    public void DecreaseMove() {
    this.currentMoves--;
    Debug.LogError($"{currentMoves}");
    }

}