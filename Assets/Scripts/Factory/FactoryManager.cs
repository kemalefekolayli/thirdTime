using UnityEngine;
using System;

public class FactoryManager : MonoBehaviour
{
    // Assign these in the Inspector
    [SerializeField] private VaseObstacleFactory vaseObstacleFactory;
    [SerializeField] private StoneObstacleFactory stoneObstacleFactory;
    [SerializeField] private BoxObstacleFactory boxObstacleFactory;
    [SerializeField] private RedCubeObjectFactory redCubeFactory;
    [SerializeField] private YellowCubeObjectFactory yellowCubeFactory;
    [SerializeField] private BlueCubeObjectFactory blueCubeFactory;
    [SerializeField] private GreenCubeObjectFactory greenCubeFactory;
    [SerializeField] private HorizontalRocketFactory horizontalRocketFactory;
    [SerializeField] private VerticalRocketFactory verticalRocketFactory;


    public ObjectFactory<IGridObject> GetFactory(string s)
    {
        switch(s)
        {
            case "hro": return horizontalRocketFactory;
            case "vro": return verticalRocketFactory;
            case "v": return vaseObstacleFactory;
            case "s": return stoneObstacleFactory;
            case "bo": return boxObstacleFactory;
            case "r": return redCubeFactory;
            case "g": return greenCubeFactory;
            case "b": return blueCubeFactory;
            case "y": return yellowCubeFactory;
            case "rand":
            System.Random random = new System.Random();
            int randomNumber = random.Next(1, 5);
            switch(randomNumber){
            case 1:
            return redCubeFactory;
            case 2:
            return greenCubeFactory;
            case 3:
            return blueCubeFactory;
            case 4:
            return yellowCubeFactory;
            default: return redCubeFactory;
            }

            default: return null;
        }
    }
}