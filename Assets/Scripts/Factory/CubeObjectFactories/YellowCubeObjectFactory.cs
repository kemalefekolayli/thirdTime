using UnityEngine;

public class YellowCubeObjectFactory : MonoBehaviour , ObjectFactory<CubeObject> {
    public GameObject cubePrefab;
    public Sprite CubeSprite;
    IGridObject CreateObject(Vector2 location, Transform parent, float cellSize, GridManager manager, Vector2Int gridPos){


        GameObject newCube = Instantiate(cubePrefab, new Vector3(location.x, location.y, 0), Quaternion.identity, parent);
        newCube.transform.localScale = Vector3.one * (cellSize * 0.7f);

        CubeObject cubeObject = newCube.GetComponent<CubeObject>();

        if( cubeObject != null ){
        cubeObject.Initialize(gridManager, gridPos);

        cubeObject.SetColor(cubeType);

        cubeObject.SetSprite(CubeSprite);

        return cubeObject;
        }

        return null;

    }
}