using UnityEngine;

public class VerticalRocketFactory : MonoBehaviour , ObjectFactory<IGridObject> {

    public GameObject VerticalRocketPrefab;
    public Sprite VerticalRocketSprite;

    public IGridObject CreateObject(Vector2 location, Transform parent, float cellSize, GridManager manager, Vector2Int gridPos){


        GameObject newRocket = Instantiate(VerticalRocketPrefab, new Vector3(location.x, location.y, 0), Quaternion.identity, parent);
        newRocket.transform.localScale = Vector3.one * (cellSize * 0.7f);

        RocketObject rocketObject = newRocket.GetComponent<RocketObject>();

        if( rocketObject != null ){
        rocketObject.Initialize(gridPos, manager);



        rocketObject.SetSprite(VerticalRocketSprite);
        rocketObject.GetComponent<SpriteRenderer>().sortingOrder = gridPos.y;

        return rocketObject;
        }

        return null;

    }

}