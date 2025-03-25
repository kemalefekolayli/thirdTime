using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameProcessQueue : MonoBehaviour
{
    private static GameProcessQueue instance;
    public static GameProcessQueue Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("GameProcessQueue");
                instance = obj.AddComponent<GameProcessQueue>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private Queue<GameProcess> processQueue = new Queue<GameProcess>();
    private bool isProcessing = false;

    public void EnqueueProcess(GameProcess process)
    {
        processQueue.Enqueue(process);
        if (!isProcessing)
        {
            ProcessNext();
        }
    }

    private void ProcessNext()
    {
        if (processQueue.Count == 0)
        {
            isProcessing = false;
            return;
        }

        isProcessing = true;
        GameProcess currentProcess = processQueue.Dequeue();
        StartCoroutine(ExecuteProcess(currentProcess));
    }

    private IEnumerator ExecuteProcess(GameProcess process)
    {
        yield return StartCoroutine(process.Execute());
        ProcessNext();
    }

    // Process abstractions that wrap existing functionality
    public abstract class GameProcess
    {
        public abstract IEnumerator Execute();
    }

    // Specific process implementations
    public class CubeMatchProcess : GameProcess
    {
        private CubeObject cubeObject;
        private CubeInputHandler inputHandler;

        public CubeMatchProcess(CubeObject cube, CubeInputHandler handler)
        {
            cubeObject = cube;
            inputHandler = handler;
        }

        public override IEnumerator Execute()
        {
            inputHandler.OnCubeClicked(cubeObject);
            // Wait until the grid is not processing
            while (GridEvents.IsProcessing)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f); // Small buffer
        }
    }

    public class RocketMatchProcess : GameProcess
    {
        private RocketObject rocketObject;
        private RocketInputHandler inputHandler;

        public RocketMatchProcess(RocketObject rocket, RocketInputHandler handler)
        {
            rocketObject = rocket;
            inputHandler = handler;
        }

        public override IEnumerator Execute()
        {
            inputHandler.OnRocketClicked(rocketObject);
            // Wait until the grid is not processing
            while (GridEvents.IsProcessing)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f); // Small buffer
        }
    }

    public class FallingProcess : GameProcess
    {
        private CubeFallingHandler fallingHandler;

        public FallingProcess(CubeFallingHandler handler)
        {
            fallingHandler = handler;
        }

        public override IEnumerator Execute()
        {
            fallingHandler.ProcessFalling();
            // Wait until the handler is no longer processing
            while (fallingHandler.IsProcessing)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f); // Small buffer
        }
    }

    public class FillingProcess : GameProcess
    {
        private GridFiller gridFiller;

        public FillingProcess(GridFiller filler)
        {
            gridFiller = filler;
        }

        public override IEnumerator Execute()
        {
            gridFiller.FillEmptySpaces();
            // Wait until the filler is no longer processing
            while (gridFiller.IsProcessing)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f); // Small buffer
        }
    }
}