using UnityEngine;
using TMPro;

public class MoveCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveCountText;
    private LevelMoveKeeper levelMoveKeeper;

    void Start()
    {
        levelMoveKeeper = FindFirstObjectByType<LevelMoveKeeper>();
        UpdateMoveDisplay();
    }

    void Update()
    {
        UpdateMoveDisplay();
    }

    private void UpdateMoveDisplay()
    {
        if (moveCountText != null && levelMoveKeeper != null)
        {
            moveCountText.text = $"{levelMoveKeeper.currentMoves}";
        }
    }
}