using UnityEngine;

public abstract class ObstacleObject : MonoBehaviour, IGridObject, IDamageable {

    protected GridManager gridManager;
    protected Vector2Int gridPosition;
    protected int health;
    protected bool isDestroyed = false;
    protected SpriteRenderer spriteRenderer;

    public bool IsDestroyed => isDestroyed;
    public bool CanFall { get; protected set; }  // Set in derived classes

        // IGridObject implementation
    public virtual void Initialize( Vector2Int position, GridManager gridManager)
    {
            this.gridManager = gridManager;
            this.gridPosition = position;
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // IDamageable implementation
    public abstract bool CanTakeDamage(DamageType damageType);

    public virtual void TakeDamage(DamageType damageType, int amount)
    {
     if (!CanTakeDamage(damageType)) return;

     health -= amount;
     if (health <= 0) {
         isDestroyed = true;
         // gridManager.RemoveObstacle(gridPosition); to be implemented later
     }
    }


}