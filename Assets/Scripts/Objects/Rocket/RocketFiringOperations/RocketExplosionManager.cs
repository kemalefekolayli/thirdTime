using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the overall rocket explosion sequence
/// </summary>
public class RocketExplosionManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CubeFallingHandler fallingHandler;
    [SerializeField] private DamageApplicator damageApplicator;
    [SerializeField] private RocketComboDetector comboDetector;

    [Header("Prefabs")]
    [SerializeField] private GameObject rocketPartPrefab;
    [SerializeField] private ParticleSystem comboExplosionPrefab;

    private int pendingRocketParts = 0;
    private HashSet<Vector2Int> affectedPositions = new HashSet<Vector2Int>();

    private void Awake()
    {
        if (gridManager == null) gridManager = Object.FindFirstObjectByType<GridManager>();
        if (fallingHandler == null) fallingHandler = Object.FindFirstObjectByType<CubeFallingHandler>();
        if (damageApplicator == null) damageApplicator = GetComponent<DamageApplicator>();
        if (comboDetector == null) comboDetector = GetComponent<RocketComboDetector>();

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
        // Check for rocket combo
        List<Vector2Int> adjacentRockets = comboDetector.FindAdjacentRockets(rocketPosition);

        if (adjacentRockets.Count > 0)
        {
            // Rocket combo detected
            HandleRocketCombo(rocketPosition, adjacentRockets);
        }
        else
        {
            // Single rocket explosion
            HandleSingleRocketExplosion(rocketPosition, rocketType);
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

        // 1. First, explode the center 3x3 area
        for (int x = mainRocketPosition.x - 1; x <= mainRocketPosition.x + 1; x++)
        {
            for (int y = mainRocketPosition.y - 1; y <= mainRocketPosition.y + 1; y++)
            {
                if (x >= 0 && x < gridManager.gridWidth && y >= 0 && y < gridManager.gridHeight)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    damageApplicator.ApplyDamage(pos, DamageType.Rocket);
                    affectedPositions.Add(pos);
                }
            }
        }

        // 2. Create rocket parts from the outer positions of the 3x3 grid
        // Corners send rockets in two directions

        // Top-left (sends up and left)
        Vector2Int topLeft = new Vector2Int(mainRocketPosition.x - 1, mainRocketPosition.y + 1);
        CreateRocketPart(topLeft, new Vector2Int(0, 1));  // Up
        CreateRocketPart(topLeft, new Vector2Int(-1, 0)); // Left

        // Top-right (sends up and right)
        Vector2Int topRight = new Vector2Int(mainRocketPosition.x + 1, mainRocketPosition.y + 1);
        CreateRocketPart(topRight, new Vector2Int(0, 1));  // Up
        CreateRocketPart(topRight, new Vector2Int(1, 0));  // Right

        // Bottom-right (sends down and right)
        Vector2Int bottomRight = new Vector2Int(mainRocketPosition.x + 1, mainRocketPosition.y - 1);
        CreateRocketPart(bottomRight, new Vector2Int(0, -1)); // Down
        CreateRocketPart(bottomRight, new Vector2Int(1, 0));  // Right

        // Bottom-left (sends down and left)
        Vector2Int bottomLeft = new Vector2Int(mainRocketPosition.x - 1, mainRocketPosition.y - 1);
        CreateRocketPart(bottomLeft, new Vector2Int(0, -1));  // Down
        CreateRocketPart(bottomLeft, new Vector2Int(-1, 0));  // Left

        // Middle edges send rockets in one direction

        // Top middle
        CreateRocketPart(new Vector2Int(mainRocketPosition.x, mainRocketPosition.y + 1), new Vector2Int(0, 1));  // Up

        // Right middle
        CreateRocketPart(new Vector2Int(mainRocketPosition.x + 1, mainRocketPosition.y), new Vector2Int(1, 0));  // Right

        // Bottom middle
        CreateRocketPart(new Vector2Int(mainRocketPosition.x, mainRocketPosition.y - 1), new Vector2Int(0, -1)); // Down

        // Left middle
        CreateRocketPart(new Vector2Int(mainRocketPosition.x - 1, mainRocketPosition.y), new Vector2Int(-1, 0)); // Left
    }

    /// <summary>
    /// Creates a rocket part that travels in the specified direction
    /// </summary>
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
        }
        else
        {
            Debug.LogError("RocketExplosionManager: Rocket part prefab does not have RocketPartController component!");
            Destroy(partObj);
        }
    }

    /// <summary>
    /// Handles the cross-pattern explosion for rocket combos
    /// </summary>
    private IEnumerator PerformComboExplosion(Vector2Int center)
    {
        // Play combo explosion effect
        if (comboExplosionPrefab != null)
        {
            Vector3 worldPos = new Vector3(
                gridManager.GridStartPos.x + center.x * gridManager.CellSize,
                gridManager.GridStartPos.y + center.y * gridManager.CellSize,
                0
            );

            ParticleSystem explosion = Instantiate(comboExplosionPrefab, worldPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f); // Wait a bit for visual effect
        }

        // Apply damage in a cross pattern (entire row AND entire column)
        List<Vector2Int> damagedPositions = new List<Vector2Int>();

        // Apply damage to entire row
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            Vector2Int pos = new Vector2Int(x, center.y);
            if (damageApplicator.ApplyDamage(pos, DamageType.Rocket))
            {
                damagedPositions.Add(pos);
            }
        }

        // Apply damage to entire column
        for (int y = 0; y < gridManager.gridHeight; y++)
        {
            // Skip the center position as it's already been damaged in the row
            if (y != center.y)
            {
                Vector2Int pos = new Vector2Int(center.x, y);
                if (damageApplicator.ApplyDamage(pos, DamageType.Rocket))
                {
                    damagedPositions.Add(pos);
                }
            }
        }

        // Add to affected positions
        foreach (Vector2Int pos in damagedPositions)
        {
            affectedPositions.Add(pos);
        }

        // If there are no pending rocket parts, finish the explosion sequence
        if (pendingRocketParts == 0)
        {
            FinishExplosionSequence();
        }
    }

    /// <summary>
    /// Called when a rocket part finishes its movement
    /// </summary>
    private void OnRocketPartFinished(List<Vector2Int> partAffectedPositions)
    {
        // Add to our global list of affected positions
        foreach (Vector2Int pos in partAffectedPositions)
        {
            affectedPositions.Add(pos);
        }

        // Decrease pending count
        pendingRocketParts--;

        // If all parts are done, finish the sequence
        if (pendingRocketParts <= 0)
        {
            FinishExplosionSequence();
        }
    }

    /// <summary>
    /// Clean up explosion and trigger falling objects
    /// </summary>
    private void FinishExplosionSequence()
    {
        // Log affected positions for debugging
        Debug.Log($"Rocket explosion affected {affectedPositions.Count} positions");

        // Reset for next explosion
        affectedPositions.Clear();

        // Trigger falling process with a slight delay to ensure all explosions are complete
        if (fallingHandler != null)
        {
            // Use a coroutine with a short delay to ensure all removal operations are complete
            StartCoroutine(TriggerFallingWithDelay(0.1f));
        }
    }

    private IEnumerator TriggerFallingWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        fallingHandler.ProcessFalling();
    }

    /// <summary>
    /// Removes a rocket from the grid
    /// </summary>
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