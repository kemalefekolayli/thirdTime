using UnityEngine;
using System.Collections.Generic;

public class VaseObstacleFactory : MonoBehaviour , ObjectFactory<VaseObstacle> {
    public GameObject obstaclePrefab;
    public Sprite ObstacleSprite;

    public IGridObject CreateObject(Vector2 location, Transform parent, float cellSize, GridManager manager, Vector2Int gridPos){


        GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(location.x, location.y, 0), Quaternion.identity, parent);
        newObstacle.transform.localScale = Vector3.one * (cellSize * 0.7f);

        ObstacleObject obstacleObject = newObstacle.GetComponent<ObstacleObject>();

        if( obstacleObject != null ){
        obstacleObject.Initialize(gridPos, manager);



        obstacleObject.SetSprite(ObstacleSprite);

        return obstacleObject;
        }

        return null;

    }
}