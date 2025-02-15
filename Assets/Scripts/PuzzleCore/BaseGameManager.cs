using UnityEngine;

public abstract class BaseGameManager<TCameraController, TWinConditionChecker> : MonoBehaviour
    where TCameraController : BaseCameraController
    where TWinConditionChecker : BaseWinConditionChecker
{
    [Header("Managers")]
    [SerializeField] protected TCameraController cameraController;
    [SerializeField] protected TWinConditionChecker winConditionChecker;
    [SerializeField] protected SceneLoader sceneLoader;

    protected abstract void SetupManagers();

    protected virtual void InitializeComponents()
    {
        if (cameraController == null)
            cameraController = GetComponent<TCameraController>();
        if (winConditionChecker == null)
            winConditionChecker = GetComponent<TWinConditionChecker>();
        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();

        CheckComponents();
    }

    protected void CheckComponents()
    {
        if (cameraController == null)
            Debug.LogError($"{typeof(TCameraController).Name} is not found!");
        if (winConditionChecker == null)
            Debug.LogError($"{typeof(TWinConditionChecker).Name} is not found!");
        if (sceneLoader == null)
            Debug.LogError("SceneLoader is not found!");
    }

    protected virtual void Awake()
    {
        InitializeComponents();
        SetupManagers();
    }
}