using UnityEngine;

public class LevelButton : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer ;
    [SerializeField] private Sprite sprite ;

    void Start(){
    LevelData level1 = LevelLoader.Instance.GetLevel(1);
            if (level1 == null)
            {
                Debug.LogError("Level data is NULL.");
                return;
            }

    }

}