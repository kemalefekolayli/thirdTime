using UnityEngine;

public enum objectColor {
r,
g,
b,
y,
rand
}


public class CubeObject : MonoBehaviour , IGridObject
{

   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private Sprite redCubeSprite;
   [SerializeField] private Sprite greenCubeSprite;
   [SerializeField] private Sprite blueCubeSprite;
   [SerializeField] private Sprite yellowCubeSprite;

   [SerializeField] private Sprite redCubeRocketHintSprite;
   [SerializeField] private Sprite greenCubeRocketHintSprite;
   [SerializeField] private Sprite blueCubeRocketHintSprite;
   [SerializeField] private Sprite yellowCubeRocketHintSprite;

   private Vector2 gridPos;
   private objectColor color;
   public bool isGrouped;

   public void Initialize(Vector2 gridPos, objectColor color){
   }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
