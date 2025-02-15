using UnityEngine;
using UnityEngine.Events;

public abstract class BaseWinConditionChecker : MonoBehaviour
{
    public UnityEvent OnWin;

    public abstract void CheckWinCondition();

    // Метод для вызова события победы
    protected void NotifyWin()
    {
        OnWin?.Invoke();
    }
}