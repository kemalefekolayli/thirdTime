using UnityEngine;

public class HorizontalRocketFactory : MonoBehaviour , ObjectFactory<IGridObject> {

    public GameObject HorizontalRocketPrefab;
    public Sprite HorizontalRocketSprite;

    public IGridObject CreateObject(Vector2 location, Transform parent, float cellSize, GridManager manager, Vector2Int gridPos){


        GameObject newRocket = Instantiate(HorizontalRocketPrefab, new Vector3(location.x, location.y, 0), Quaternion.identity, parent);
        newRocket.transform.localScale = Vector3.one * (cellSize * 0.7f);

        RocketObject rocketObject = newRocket.GetComponent<RocketObject>();

        if( rocketObject != null ){
        rocketObject.Initialize(gridPos, manager);



        rocketObject.SetSprite(HorizontalRocketSprite);
        rocketObject.GetComponent<SpriteRenderer>().sortingOrder = gridPos.y;

        return rocketObject;
        }

        return null;

    }

}