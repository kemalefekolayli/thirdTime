using UnityEngine;

public class BoxObstacle : ObstacleObject, IGridObject {
    [SerializeField] private Sprite defaultSprite;



    private void Awake(){
        health = 1;
        CanFall = false;
    }

    public override void Initialize(Vector2Int position, GridManager gridManager)
    {
        base.Initialize(position, gridManager);

        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
    // Box takes damage from both adjacent blasts and rockets
    public override bool CanTakeDamage(DamageType damageType)
    {
        return true;
    }

    protected void UpdateVisuals()
    {
        // Box has only 1 health, so no need for damaged visuals
    }


}