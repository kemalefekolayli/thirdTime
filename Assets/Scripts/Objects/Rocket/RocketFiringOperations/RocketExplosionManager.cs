using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketExplosionManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CubeFallingHandler fallingHandler;
    [SerializeField] private DamageApplicator damageApplicator;
    [SerializeField] private RocketComboDetector comboDetector;
    private GridFiller gridFiller;
    [SerializeField] private GoalTracker goalTracker;

    [Header("Prefabs")]
    [SerializeField] private GameObject rocketPartPrefab;
    [SerializeField] private ParticleSystem comboExplosionPrefab;

    private int pendingRocketParts = 0;
    private HashSet<Vector2Int> affectedPositions = new HashSet<Vector2Int>();
    private bool isProcessing = false;

    // Add a public property to check if explosion is in progress
    public bool IsProcessing => isProcessing;

    private void Awake()
    {
        if (gridManager == null) gridManager = Object.FindFirstObjectByType<GridManager>();
        if (fallingHandler == null) fallingHandler = Object.FindFirstObjectByType<CubeFallingHandler>();
        if (damageApplicator == null) damageApplicator = GetComponent<DamageApplicator>();
        if (comboDetector == null) comboDetector = GetComponent<RocketComboDetector>();
        if (gridFiller == null) gridFiller = Object.FindFirstObjectByType<GridFiller>();

        if (gridManager == null) Debug.LogError("RocketExplosionManager: GridManager reference not found!");
        if (fallingHandler == null) Debug.LogError("RocketExplosionManager: CubeFallingHandler reference not found!");
        if (damageApplicator == null) Debug.LogError("RocketExplosionManager: DamageApplicator reference not found!");
        if (comboDetector == null) Debug.LogError("RocketExplosionManager: RocketComboDetector reference not found!");
    }

    /// <summary>
    /// Start the explosion sequence for a clicked rocket
    /// </summary>
    public void ExplodeRocket(Vector2Int rocketPosition, string rocketType)
    {
        isProcessing = true;
        Debug.Log($"Starting rocket explosion at {rocketPosition}, type: {rocketType}");

        // Reset counters for new explosion sequence
        pendingRocketParts = 0;
        affectedPositions.Clear();

        // Check for rocket combo
        List<Vector2Int> adjacentRockets = comboDetector.FindAdjacentRockets(rocketPosition);

        if (adjacentRockets.Count > 0)
        {
            // Rocket combo detected
            Debug.Log($"Rocket combo detected with {adjacentRockets.Count} adjacent rockets");
            HandleRocketCombo(rocketPosition, adjacentRockets);
            goalTracker.UpdateGoals();
        }
        else
        {
            // Single rocket explosion
            Debug.Log("Single rocket explosion");
            HandleSingleRocketExplosion(rocketPosition, rocketType);
            goalTracker.UpdateGoals();
        }

        // Check if we actually created any rocket parts
        if (pendingRocketParts == 0)
        {
            Debug.Log("No rocket parts created, finishing explosion sequence immediately");
            FinishExplosionSequence();
            goalTracker.UpdateGoals();
        }
    }

    /// <summary>
    /// Handle explosion for a single rocket (no combo)
    /// </summary>
    private void HandleSingleRocketExplosion(Vector2Int rocketPosition, string rocketType)
    {
        // Determine if horizontal or vertical rocket
        bool isHorizontal = rocketType == "hro";

        // Create two rocket parts moving in opposite directions
        if (isHorizontal)
        {
            CreateRocketPart(rocketPosition, new Vector2Int(-1, 0)); // Left
            CreateRocketPart(rocketPosition, new Vector2Int(1, 0));  // Right
        }
        else
        {
            CreateRocketPart(rocketPosition, new Vector2Int(0, -1)); // Down
            CreateRocketPart(rocketPosition, new Vector2Int(0, 1));  // Up
        }
    }

    /// <summary>
    /// Handle explosive combo of multiple rockets
    /// </summary>
    private void HandleRocketCombo(Vector2Int mainRocketPosition, List<Vector2Int> otherRockets)
    {
        // Remove all involved rockets from the grid
        foreach (Vector2Int pos in otherRockets)
        {
            RemoveRocketAt(pos);
        }

        // Create visual effects at the center
        Vector3 centerWorldPos = new Vector3(
            gridManager.GridStartPos.x + mainRocketPosition.x * gridManager.CellSize,
            gridManager.GridStartPos.y + mainRocketPosition.y * gridManager.CellSize,
            0
        );

        if (comboExplosionPrefab != null)
        {
            Instantiate(comboExplosionPrefab, centerWorldPos, Quaternion.identity);
        }

        // Apply damage to the 3x3 center area immediately
        for (int x = mainRocketPosition.x - 1; x <= mainRocketPosition.x + 1; x++)
        {
            for (int y = mainRocketPosition.y - 1; y <= mainRocketPosition.y + 1; y++)
            {
                if (x >= 0 && x < gridManager.gridWidth && y >= 0 && y < gridManager.gridHeight)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (damageApplicator.ApplyDamage(pos, DamageType.Rocket))
                    {
                        affectedPositions.Add(pos);
                    }
                }
            }
        }

        // Create rocket parts from corners and edges
        // Carefully track the number of rocket parts created
        int createdRocketParts = 0;

        // Top-left (sends up and left)
        if (IsValidCoordinate(mainRocketPosition.x - 1, mainRocketPosition.y + 1))
        {
            Vector2Int topLeft = new Vector2Int(mainRocketPosition.x - 1, mainRocketPosition.y + 1);
            CreateRocketPart(topLeft, new Vector2Int(0, 1));  // Up
            CreateRocketPart(topLeft, new Vector2Int(-1, 0)); // Left
            createdRocketParts += 2;
        }

        // Top-right (sends up and right)
        if (IsValidCoordinate(mainRocketPosition.x + 1, mainRocketPosition.y + 1))
        {
            Vector2Int topRight = new Vector2Int(mainRocketPosition.x + 1, mainRocketPosition.y + 1);
            CreateRocketPart(topRight, new Vector2Int(0, 1));  // Up
            CreateRocketPart(topRight, new Vector2Int(1, 0));  // Right
            createdRocketParts += 2;
        }

        // Bottom-right (sends down and right)
        if (IsValidCoordinate(mainRocketPosition.x + 1, mainRocketPosition.y - 1))
        {
            Vector2Int bottomRight = new Vector2Int(mainRocketPosition.x + 1, mainRocketPosition.y - 1);
            CreateRocketPart(bottomRight, new Vector2Int(0, -1)); // Down
            CreateRocketPart(bottomRight, new Vector2Int(1, 0));  // Right
            createdRocketParts += 2;
        }

        // Bottom-left (sends down and left)
        if (IsValidCoordinate(mainRocketPosition.x - 1, mainRocketPosition.y - 1))
        {
            Vector2Int bottomLeft = new Vector2Int(mainRocketPosition.x - 1, mainRocketPosition.y - 1);
            CreateRocketPart(bottomLeft, new Vector2Int(0, -1));  // Down
            CreateRocketPart(bottomLeft, new Vector2Int(-1, 0));  // Left
            createdRocketParts += 2;
        }

        // Middle edges send rockets in one direction
        // Top middle
        if (IsValidCoordinate(mainRocketPosition.x, mainRocketPosition.y + 1))
        {
            CreateRocketPart(new Vector2Int(mainRocketPosition.x, mainRocketPosition.y + 1), new Vector2Int(0, 1));  // Up
            createdRocketParts++;
        }

        // Right middle
        if (IsValidCoordinate(mainRocketPosition.x + 1, mainRocketPosition.y))
        {
            CreateRocketPart(new Vector2Int(mainRocketPosition.x + 1, mainRocketPosition.y), new Vector2Int(1, 0));  // Right
            createdRocketParts++;
        }

        // Bottom middle
        if (IsValidCoordinate(mainRocketPosition.x, mainRocketPosition.y - 1))
        {
            CreateRocketPart(new Vector2Int(mainRocketPosition.x, mainRocketPosition.y - 1), new Vector2Int(0, -1)); // Down
            createdRocketParts++;
        }

        // Left middle
        if (IsValidCoordinate(mainRocketPosition.x - 1, mainRocketPosition.y))
        {
            CreateRocketPart(new Vector2Int(mainRocketPosition.x - 1, mainRocketPosition.y), new Vector2Int(-1, 0)); // Left
            createdRocketParts++;
        }

        Debug.Log($"Created {createdRocketParts} rocket parts for combo explosion");

        // If somehow we didn't create any rocket parts, we need to finish manually
        if (createdRocketParts == 0)
        {
            FinishExplosionSequence();
        }
    }

    // Helper method to check if coordinates are within grid bounds
    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < gridManager.gridWidth && y >= 0 && y < gridManager.gridHeight;
    }


    private void CreateRocketPart(Vector2Int startPosition, Vector2Int direction)
    {
        if (rocketPartPrefab == null)
        {
            Debug.LogError("RocketExplosionManager: Rocket part prefab is not assigned!");
            return;
        }

        // Create the rocket part
        GameObject partObj = Instantiate(rocketPartPrefab, transform);
        RocketPartController partController = partObj.GetComponent<RocketPartController>();

        if (partController != null)
        {
            // Initialize the part
            partController.Initialize(gridManager, startPosition, direction, damageApplicator);

            // Subscribe to its completion event
            partController.OnFinished += OnRocketPartFinished;

            // Track pending rocket parts
            pendingRocketParts++;
            Debug.Log($"Created rocket part moving {direction}, total pending: {pendingRocketParts}");
        }
        else
        {
            Debug.LogError("RocketExplosionManager: Rocket part prefab does not have RocketPartController component!");
            Destroy(partObj);
        }
    }


    private void OnRocketPartFinished(List<Vector2Int> partAffectedPositions)
    {
        // Add to our global list of affected positions
        foreach (Vector2Int pos in partAffectedPositions)
        {
            affectedPositions.Add(pos);
        }

        // Decrease pending count
        pendingRocketParts--;
        Debug.Log($"Rocket part finished, remaining parts: {pendingRocketParts}");

        // If all parts are done, finish the sequence
        if (pendingRocketParts <= 0)
        {
            FinishExplosionSequence();
        }
    }

    private void FinishExplosionSequence()
    {
        // Log affected positions for debugging
        Debug.Log($"Rocket explosion affected {affectedPositions.Count} positions, finishing sequence");

        // Reset for next explosion
        affectedPositions.Clear();

        // Use a coroutine with a short delay to ensure all removal operations are complete
        StartCoroutine(TriggerFallingWithDelay(0.2f));
    }

    private IEnumerator TriggerFallingWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (fallingHandler != null)
        {
            Debug.Log("Triggering falling sequence after rocket explosion");
            fallingHandler.ProcessFalling();
        }

        // Mark explosion as complete AFTER triggered falling
        isProcessing = false;
    }


    private void RemoveRocketAt(Vector2Int position)
    {
        if (gridManager.Storage.HasObjectAt(position))
        {
            MonoBehaviour mb = gridManager.Storage.GetObjectAt(position) as MonoBehaviour;
            if (mb != null)
            {
                gridManager.Storage.RemoveObject(position);
                Destroy(mb.gameObject);
            }
        }
    }
}