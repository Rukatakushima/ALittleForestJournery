using UnityEngine;

public abstract class BaseGameManager<TLevelSpawner, TCameraController, TWinConditionChecker, TLevelData> : MonoBehaviour
    where TLevelSpawner : BaseLevelSpawner
    where TCameraController : BaseCameraController
    where TWinConditionChecker : BaseWinConditionChecker
    where TLevelData : ScriptableObject
{
    [Header("Managers")]
    [SerializeField] protected TLevelSpawner levelSpawner;
    [SerializeField] protected TCameraController cameraController;
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
            cameraController = GetComponent<TCameraController>();
        if (winConditionChecker == null)
            winConditionChecker = GetComponent<TWinConditionChecker>();
        winConditionChecker.OnWin.AddListener(GameFinished);
        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();
        if (inputHandler == null)
            inputHandler = GetComponent<InputHandler>();

        CheckComponents();
    }

    protected void CheckComponents()
    {
        if (cameraController == null)
            Debug.LogError("CameraController is not found!");
        if (winConditionChecker == null)
            Debug.LogError("WinConditionChecker is not found!");
        if (sceneLoader == null)
            Debug.LogError("SceneLoader is not found!");
        if (inputHandler == null)
            Debug.LogError("InputHandler is not found!");
    }

    protected abstract void SetupManagers();

    protected virtual void SetupInputHandlers()
    {
        inputHandler.OnMouseDown.AddListener(HandleMouseDown);
        inputHandler.OnMouseDrag.AddListener(HandleMouseDrag);
        inputHandler.OnMouseUp.AddListener(HandleMouseUp);
    }

    protected abstract void HandleMouseDown(Vector2 mousePosition);

    protected abstract void HandleMouseDrag(Vector2 mousePosition);

    protected abstract void HandleMouseUp();

    protected void CheckWinCondition()
    {
        if (winConditionChecker != null)
        {
            winConditionChecker.CheckWinCondition();
        }
        else
        {
            Debug.LogError("WinConditionChecker is not assigned!");
        }
    }

    protected void GameFinished()
    {
        if (sceneLoader != null)
        {
            sceneLoader.ChangeScene();
        }
        else
        {
            Debug.LogError("SceneLoader is not assigned!");
        }
    }
}