using UnityEngine;
using TMPro;
public class GoalTracker : MonoBehaviour {

    private int VaseCount;
    private int BoxCount;
    private int StoneCount;
    [SerializeField] private GameObject boxGoalPrefab;
    [SerializeField] private GameObject vaseGoalPrefab;
    [SerializeField] private GameObject stoneGoalPrefab;
    [SerializeField] private Transform goalContainer;

    void Start() {
        LevelData level1 = LevelLoader.Instance.GetLevel(1);

        if (level1 == null) {
            Debug.LogError("Level data is NULL.");
            return;
        }


        string[] grid = level1.grid;
        VaseCount = 0;
        BoxCount = 0;
        StoneCount = 0;
        int cont = 0;

        for (int i = 0; i < grid.Length; i++) {
            switch (grid[i]) {
                case "bo": BoxCount++; break;
                case "v": VaseCount++; break;
                case "s": StoneCount++; break;
                default: cont++; break;
            }
        }
        if (BoxCount > 0) {
        GameObject boxGoal = Instantiate(boxGoalPrefab, goalContainer);
        boxGoal.GetComponentInChildren<TextMeshProUGUI>().text = BoxCount.ToString();
        }

        if (VaseCount > 0) {
        GameObject vaseGoal = Instantiate(vaseGoalPrefab, goalContainer);
        vaseGoal.GetComponentInChildren<TextMeshProUGUI>().text = BoxCount.ToString();
        }

        if (StoneCount > 0) {
        GameObject stoneGoal = Instantiate(stoneGoalPrefab, goalContainer);
        stoneGoal.GetComponentInChildren<TextMeshProUGUI>().text = BoxCount.ToString();
         }
    }
}
