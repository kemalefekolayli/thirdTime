using UnityEngine;

public class StoneObstacle : ObstacleObject , IGridObject {

    [SerializeField] private Sprite defaultSprite;

    private void Awake()
    {
        health = 1;
        CanFall = false;
    }

    public override void Initialize( Vector2Int position, GridManager gridManager)
    {
        base.Initialize( position, gridManager);

        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    // Stone only takes damage from rockets
    public override bool CanTakeDamage(DamageType damageType)
    {
        return damageType == DamageType.Rocket;
    }


}