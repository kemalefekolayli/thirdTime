using System;
using UnityEngine;

public interface IGridObject {

public void Initialize(Vector2Int gridPosition, GridManager gridManager){}

public void SetSprite(Sprite sprite){   }

}