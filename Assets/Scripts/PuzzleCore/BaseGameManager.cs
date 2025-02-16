using UnityEngine;

public abstract class BaseGameManager<TCameraController, TWinConditionChecker, TLevelData> : MonoBehaviour
    where TCameraController : BaseCameraController
    where TWinConditionChecker : BaseWinConditionChecker
    where TLevelData : ScriptableObject
{
    [Header("Managers")]
    [SerializeField] protected TCameraController cameraController;
    [SerializeField] protected TWinConditionChecker winConditionChecker;
    [SerializeField] protected SceneLoader sceneLoader;
    [SerializeField] protected InputHandler inputHandler;

    [Header("Game Settings")]
    [SerializeField] protected TLevelData level;

    protected abstract void SetupManagers();

    protected virtual void InitializeComponents()
    {
        if (cameraController == null)
            cameraController = GetComponent<TCameraController>();
        if (winConditionChecker == null)
            winConditionChecker = GetComponent<TWinConditionChecker>();
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

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        InitializeComponents();
        SpawnLevel();
        SetupManagers();
        SetupInputHandlers();
    }

    protected abstract void SpawnLevel();

    protected virtual void SetupInputHandlers()
    {
        inputHandler.OnMouseDown.AddListener(MouseDown);
        inputHandler.OnMouseDrag.AddListener(MouseDrag);
        inputHandler.OnMouseUp.AddListener(OnMouseUp);
    }

    protected abstract void MouseDown(Vector2 mousePosition);

    protected abstract void MouseDrag(Vector2 mousePosition);

    protected abstract void OnMouseUp();
}