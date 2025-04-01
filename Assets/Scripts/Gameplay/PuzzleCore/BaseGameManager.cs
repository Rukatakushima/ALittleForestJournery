using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameManager : MonoBehaviour
{
    [SerializeField] protected InputHandler inputHandler;
    public UnityEvent OnAwake, OnWin;

    protected virtual void Awake()
    {
        OnAwake?.Invoke();
        SetupInputHandlers();
    }

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

    public abstract void CheckWinCondition();
}