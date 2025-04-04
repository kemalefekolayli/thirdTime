using UnityEngine;

public class VerticalRocket : RocketObject  {

    [SerializeField] private Sprite RocketSprite;

    public override void Initialize(Vector2Int position, GridManager gridManager)
        {
            base.Initialize(position, gridManager);

            if (base.GetSpriteRenderer() != null && RocketSprite != null)
            {
                SetSprite(RocketSprite);
            }
        }



}