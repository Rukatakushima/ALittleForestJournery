using UnityEngine;
using UnityEngine.Events;
// using UnityEngine.Events;

public abstract class BaseGameManager/*<TLevelSpawner, DefaultCameraController, TWinConditionChecker, TLevelData>*/ : MonoBehaviour
/*where TLevelSpawner : BaseLevelSpawner<TLevelData>*/
// where TWinConditionChecker : BaseWinConditionChecker
/*where TLevelData : ScriptableObject*/
{
    // [Header("Managers")]
    /*[SerializeField]*/
    // protected TLevelSpawner levelSpawner;
    /*[SerializeField]*/
    // protected DefaultCameraController cameraController;
    /*[SerializeField]*/
    // protected TWinConditionChecker winConditionChecker;
    [SerializeField] protected InputHandler inputHandler;
    public UnityEvent OnAwake, OnWin;

    // [Header("Game Settings")]
    // /*[SerializeField]*/ public TLevelData level;

    // public UnityEvent OnGameInitialized = new();

    protected virtual void Awake()
    {
        // InitializeComponents();
        OnAwake?.Invoke();
        // levelSpawner?.SpawnLevel();
        // SetupManagers();
        // OnGameInitialized?.Invoke();
        SetupInputHandlers();
    }

    protected virtual void InitializeComponents()
    {
        // if (levelSpawner == null)
        //     levelSpawner = GetComponent<TLevelSpawner>();
        // if (cameraController == null)
        //     cameraController = GetComponent<DefaultCameraController>();
        // if (winConditionChecker == null)
        //     winConditionChecker = GetComponent<TWinConditionChecker>();
        if (inputHandler == null)
            inputHandler = GetComponent<InputHandler>();

        CheckComponents();
    }

    protected void CheckComponents()
    {
        // if (levelSpawner == null)
        //     throw new System.ArgumentNullException(nameof(levelSpawner));
        // if (cameraController == null)
        //     throw new System.ArgumentNullException(nameof(cameraController));
        // if (winConditionChecker == null)
        //     throw new System.ArgumentNullException(nameof(winConditionChecker));
        if (inputHandler == null)
            throw new System.ArgumentNullException(nameof(inputHandler));
    }

    // protected abstract void SetupManagers();

    protected virtual void SetupInputHandlers()
    {
        if (inputHandler == null)
        {
            inputHandler = GetComponent<InputHandler>();
            if (inputHandler == null)
                throw new System.ArgumentNullException(nameof(inputHandler));
        }

        inputHandler.OnInputStart.AddListener(HandleInputStart);
        inputHandler.OnInputUpdate.AddListener(HandleInputUpdate);
        inputHandler.OnInputEnd.AddListener(HandleInputEnd);
    }

    protected abstract void HandleInputStart(Vector2 mousePosition);

    protected abstract void HandleInputUpdate(Vector2 mousePosition);

    protected abstract void HandleInputEnd();

    // public void CheckWinCondition()
    // {
    //     if (winConditionChecker != null)
    //         winConditionChecker.CheckWinCondition();
    //     else
    //         Debug.LogError("WinConditionChecker is not assigned!");
    // }

    public abstract void CheckWinCondition();
    // protected void NotifyWin() => OnWin?.Invoke();
}