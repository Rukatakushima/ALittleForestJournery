using UnityEngine;
using UnityEngine.Events;

public abstract class BaseWinConditionChecker : MonoBehaviour
{
    public UnityEvent OnWin;

    public abstract void CheckWinCondition();

    protected void NotifyWin() => OnWin?.Invoke();
}