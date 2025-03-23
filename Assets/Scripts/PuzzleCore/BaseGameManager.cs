using UnityEngine;

public abstract class BaseGameManager<TLevelSpawner, DefaultCameraController, TWinConditionChecker, TLevelData> : MonoBehaviour
    where TLevelSpawner : BaseLevelSpawner
    where TWinConditionChecker : BaseWinConditionChecker
    where TLevelData : ScriptableObject
{
    [Header("Managers")]
    [SerializeField] protected TLevelSpawner levelSpawner;
    [SerializeField] protected DefaultCameraController cameraController;
    [SerializeField] protected TWinConditionChecker winConditionChecker;
    [SerializeField] protected SceneLoader sceneLoader;
    [SerializeField] protected InputHandler inputHandler;

    [Header("Game Settings")]
    [SerializeField] public TLevelData level;

    protected virtual void Awake()
    {
        InitializeComponents();
        SetupManagers();
        SetupInputHandlers();
    }

    protected virtual void InitializeComponents()
    {
        if (levelSpawner == null)
            levelSpawner = GetComponent<TLevelSpawner>();
        if (cameraController == null)
            cameraController = GetComponent<DefaultCameraController>();
        if (winConditionChecker == null)
            winConditionChecker = GetComponent<TWinConditionChecker>();
        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();
        if (inputHandler == null)
            inputHandler = GetComponent<InputHandler>();

        winConditionChecker.OnWin.AddListener(GameFinished);

        CheckComponents();
    }

    protected void CheckComponents()
    {
        if (cameraController == null)
            throw new System.ArgumentNullException(nameof(cameraController));
        if (winConditionChecker == null)
            throw new System.ArgumentNullException(nameof(winConditionChecker));
        if (sceneLoader == null)
            throw new System.ArgumentNullException(nameof(sceneLoader));
        if (inputHandler == null)
            throw new System.ArgumentNullException(nameof(inputHandler));
    }

    protected abstract void SetupManagers();

    protected virtual void SetupInputHandlers()
    {
        inputHandler.OnInputStart.AddListener(HandleInputStart);
        inputHandler.OnInputUpdate.AddListener(HandleInputUpdate);
        inputHandler.OnInputEnd.AddListener(HandleInputEnd);
    }

    protected abstract void HandleInputStart(Vector2 mousePosition);

    protected abstract void HandleInputUpdate(Vector2 mousePosition);

    protected abstract void HandleInputEnd();

    public void CheckWinCondition()
    {
        if (winConditionChecker != null)
            winConditionChecker.CheckWinCondition();
        else
            Debug.LogError("WinConditionChecker is not assigned!");
    }

    protected void GameFinished()
    {
        if (sceneLoader != null)
            sceneLoader.ChangeScene();
        else
            Debug.LogError("SceneLoader is not assigned!");
    }
}