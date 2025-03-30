using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor-only class that adds menu items to set the current level number.
/// </summary>
public class LevelEditorMenu : Editor
{
    private const int MaxLevel = 10;

    [MenuItem("Dream Games/Set Level/Level 1")]
    private static void SetLevel1() { SetLevelNumber(1); }

    [MenuItem("Dream Games/Set Level/Level 2")]
    private static void SetLevel2() { SetLevelNumber(2); }

    [MenuItem("Dream Games/Set Level/Level 3")]
    private static void SetLevel3() { SetLevelNumber(3); }

    [MenuItem("Dream Games/Set Level/Level 4")]
    private static void SetLevel4() { SetLevelNumber(4); }

    [MenuItem("Dream Games/Set Level/Level 5")]
    private static void SetLevel5() { SetLevelNumber(5); }

    [MenuItem("Dream Games/Set Level/Level 6")]
    private static void SetLevel6() { SetLevelNumber(6); }

    [MenuItem("Dream Games/Set Level/Level 7")]
    private static void SetLevel7() { SetLevelNumber(7); }

    [MenuItem("Dream Games/Set Level/Level 8")]
    private static void SetLevel8() { SetLevelNumber(8); }

    [MenuItem("Dream Games/Set Level/Level 9")]
    private static void SetLevel9() { SetLevelNumber(9); }

    [MenuItem("Dream Games/Set Level/Level 10")]
    private static void SetLevel10() { SetLevelNumber(10); }

    [MenuItem("Dream Games/Set Level/All Levels Finished")]
    private static void SetLevelFinished() { SetLevelNumber(MaxLevel + 1); }

    [MenuItem("Dream Games/Reset Progress")]
    private static void ResetProgress()
    {
        SetLevelNumber(1);
    }

    /// <summary>
    /// Sets the level number in PlayerPrefs directly, with validation.
    /// </summary>
    private static void SetLevelNumber(int level)
    {
        if (level < 1)
        {
            Debug.LogWarning("Cannot set level below 1. Setting to level 1.");
            level = 1;
        }

        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();
        Debug.Log($"Level set to: {level}");
    }
}