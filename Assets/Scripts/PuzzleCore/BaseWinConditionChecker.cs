using UnityEngine;
using UnityEngine.Events;

public abstract class BaseWinConditionChecker : MonoBehaviour
{
    public UnityEvent OnWin;

    public abstract bool isWinConditionFulfilled();

    // Метод для вызова события победы
    protected void NotifyWin()
    {
        OnWin?.Invoke();
    }
}