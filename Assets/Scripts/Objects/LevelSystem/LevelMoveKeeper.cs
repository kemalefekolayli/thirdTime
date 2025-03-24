using UnityEngine;


public class LevelMoveKeeper : MonoBehaviour {


    public int movesLeft;
    public int maxMoves;



    void Start(){
    LevelData level1 = LevelLoader.Instance.GetLevel(1);

    if (level1 == null)
            {
                Debug.LogError("Level data is NULL.");
                return;
            }
    maxMoves = level1.move_count;
    movesLeft = level1.move_count;
    }

    void Update(){

    }

}