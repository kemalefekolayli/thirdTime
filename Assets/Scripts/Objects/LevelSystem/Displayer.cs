using UnityEngine;
using TMPro;

public class MyDisplayScript : MonoBehaviour
{
    public TMP_Text infoText;
    public LevelMoveKeeper moveKeeper;
    void Update()
    {
        infoText.text = $"{moveKeeper.movesLeft}" ;
    }
}
