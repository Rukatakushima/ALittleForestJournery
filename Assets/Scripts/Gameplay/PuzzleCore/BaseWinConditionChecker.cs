using UnityEngine;
using UnityEngine.Events;

public abstract class BaseWinConditionChecker : MonoBehaviour
{
    public UnityEvent OnWin;

    private void Start() => Initialize();
    public abstract void Initialize();
    public abstract void CheckWinCondition();
    protected void NotifyWin() => OnWin?.Invoke();
}