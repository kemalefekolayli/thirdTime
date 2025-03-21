using UnityEngine;

public abstract class RocketObject : MonoBehaviour, IGridObject {

        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector2Int gridPosition;
        private GridManager gridManager;
        public bool isGrouped;

    public void Initialize(Vector2Int gridPosition, GridManager gridManager){
        this.gridManager = gridManager ;
        this.gridPosition = gridPosition;

    }

    public void SetSprite(Sprite sprite){
    if (spriteRenderer == null){
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
             }







}
}