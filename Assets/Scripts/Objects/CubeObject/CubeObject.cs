using UnityEngine;
using System.Collections.Generic;

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

   private Vector2Int gridPos;
   private objectColor color;
   private GridManager gridManager;
   public bool isGrouped;

   public objectColor GetCubeColor(){
   return this.color;
   }

   public void Initialize(Vector2Int gridPos,GridManager manager){
           this.gridManager = manager;
           this.gridPos = gridPos;
   }
    void Start()
    {
        
    }
    public void OnMouseDown(){
    Debug.LogError(this.color);
    }

    public void SetSprite(Sprite sprite)
     {
                if (spriteRenderer == null)
                    spriteRenderer = GetComponent<SpriteRenderer>();

                spriteRenderer.sprite = sprite;
     }
    public void SetColor(string colorT)
    {

        if (colorT == "r")
            color = objectColor.r;
        else if (colorT == "g")
            color = objectColor.g;
        else if (colorT == "b")
            color = objectColor.b;
        else if (colorT == "y")
            color = objectColor.y;
        else if (colorT == "rand")
        {
            // Choose a random color
            color = (objectColor)Random.Range(0, 4);
        }
        else {
        Debug.LogWarning($"Invalid color: {color}");
        color = objectColor.r;
        }
    }
    public void SetRocketHintVisible(bool visible)
    {
        if (visible)
        {
            // Choose the right rocket hint sprite based on color
            switch (color)
            {
                case objectColor.r:
                    SetSprite(redCubeRocketHintSprite);
                    break;
                case objectColor.g:
                    SetSprite(greenCubeRocketHintSprite);
                    break;
                case objectColor.b:
                    SetSprite(blueCubeRocketHintSprite);
                    break;
                case objectColor.y:
                    SetSprite(yellowCubeRocketHintSprite);
                    break;
            }
        }
        else
        {
            // Restore normal sprite
            switch (color)
            {
                case objectColor.r:
                    SetSprite(redCubeSprite);
                    break;
                case objectColor.g:
                    SetSprite(greenCubeSprite);
                    break;
                case objectColor.b:
                    SetSprite(blueCubeSprite);
                    break;
                case objectColor.y:
                    SetSprite(yellowCubeSprite);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
