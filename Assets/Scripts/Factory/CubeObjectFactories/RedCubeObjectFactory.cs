using UnityEngine;
using System.Collections.Generic;

public class RedCubeObjectFactory : MonoBehaviour , ObjectFactory<CubeObject> {
    public GameObject cubePrefab;
    public Sprite CubeSprite;
    public IGridObject CreateObject(Vector2 location, Transform parent, float cellSize, GridManager manager, Vector2Int gridPos){


        GameObject newCube = Instantiate(cubePrefab, new Vector3(location.x, location.y, 0), Quaternion.identity, parent);
        newCube.transform.localScale = Vector3.one * (cellSize * 0.7f);

        CubeObject cubeObject = newCube.GetComponent<CubeObject>();

        if( cubeObject != null ){
        cubeObject.Initialize(gridPos, manager);

        cubeObject.SetColor("r");

        cubeObject.SetSprite(CubeSprite);

        return cubeObject;
        }

        return null;

    }
}