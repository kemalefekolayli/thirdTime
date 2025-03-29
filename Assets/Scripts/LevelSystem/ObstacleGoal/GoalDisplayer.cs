using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GoalDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI StoneText;
    [SerializeField] private TextMeshProUGUI VaseText;
    [SerializeField] private TextMeshProUGUI BoxText;

    [SerializeField] private GameObject StoneContainer;
    [SerializeField] private GameObject VaseContainer;
    [SerializeField] private GameObject BoxContainer;

    private GoalTracker goalTracker;
    private bool hasInitialized = false;

    void Start()
    {
        Debug.Log("GoalDisplayer Start called");

        // Hide all containers initially
        if (StoneContainer != null) StoneContainer.SetActive(false);
        if (VaseContainer != null) VaseContainer.SetActive(false);
        if (BoxContainer != null) BoxContainer.SetActive(false);

        // Wait for other systems to initialize
        Invoke("DelayedInitialization", 0.5f);
    }

    private void DelayedInitialization()
    {
        Debug.Log("GoalDisplayer DelayedInitialization called");
        if (hasInitialized) return;
        hasInitialized = true;

        goalTracker = FindFirstObjectByType<GoalTracker>();
        if (goalTracker == null)
        {
            Debug.LogError("GoalDisplayer: Could not find GoalTracker!");
            return;
        }

        // Let GoalTracker handle the initial counts and UI update
        // This prevents multiple counted calls
    }

    public void DisplayGoals(Dictionary<string, int> obstacles)
    {
        Debug.Log("GoalDisplayer DisplayGoals called");

        // Stone goal display
        if (obstacles.ContainsKey("s") && obstacles["s"] > 0)
        {
            Debug.Log($"Stone count: {obstacles["s"]}");
            if (StoneContainer != null)
            {
                StoneContainer.SetActive(true);
                if (StoneText != null) StoneText.text = obstacles["s"].ToString();
            }
        }
        else
        {
            if (StoneContainer != null) StoneContainer.SetActive(false);
        }

        // Vase goal display
        if (obstacles.ContainsKey("v") && obstacles["v"] > 0)
        {
            Debug.Log($"Vase count: {obstacles["v"]}");
            if (VaseContainer != null)
            {
                VaseContainer.SetActive(true);
                if (VaseText != null) VaseText.text = obstacles["v"].ToString();
            }
        }
        else
        {
            if (VaseContainer != null) VaseContainer.SetActive(false);
        }

        // Box goal display
        if (obstacles.ContainsKey("bo") && obstacles["bo"] > 0)
        {
            Debug.Log($"Box count: {obstacles["bo"]}");
            if (BoxContainer != null)
            {
                BoxContainer.SetActive(true);
                if (BoxText != null) BoxText.text = obstacles["bo"].ToString();
            }
        }
        else
        {
            if (BoxContainer != null) BoxContainer.SetActive(false);
        }
    }
}