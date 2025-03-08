using UnityEngine;
using UnityEngine.Events;

public interface IInputHandler
{
    UnityEvent<Vector2> OnInputStart { get; }
    UnityEvent<Vector2> OnInputUpdate { get; }
    UnityEvent OnInputEnd { get; }
}