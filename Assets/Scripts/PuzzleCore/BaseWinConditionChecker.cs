using UnityEngine;

public abstract class BaseWinConditionChecker : MonoBehaviour
{
    // protected bool hasGameFinished = false;
    public event System.Action OnWin;
    [SerializeField] private SceneLoader sceneLoader;

    protected void Awake()
    {
        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();
    }

    public abstract bool CheckWinCondition();

    // Метод для обновления состояния (вызывается извне, например, после хода игрока)
    public void UpdateWinCondition()
    {
        if (CheckWinCondition())
        {
            OnWin?.Invoke(); // Уведомляем о победе
        }
    }
    
    // protected void CheckHasGameFinished()
    // {
    //     if (hasGameFinished)
    //     {
    //         sceneLoader.LoadScene();
    //     }
    // }

    // protected void SetHasGameFinishedTrue()
    // {
    //     hasGameFinished = true;
    // }
}