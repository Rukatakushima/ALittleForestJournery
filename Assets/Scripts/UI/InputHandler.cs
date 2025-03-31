using UnityEngine;
using UnityEngine.Events;

public enum InputType
{
    Mouse,
    Keyboard
}

public class InputHandler : MonoBehaviour//, IInputHandler
{

    [SerializeField] private InputType inputType = InputType.Mouse;

    [Header("Keyboard Input Settings")]
    [SerializeField] private KeyCode startKey = KeyCode.E;
    [SerializeField] private KeyCode updateKey = KeyCode.E;
    [SerializeField] private KeyCode endKey = KeyCode.Q;
    [SerializeField] private Transform targetObject;

    [Header("Events")]
    public UnityEvent<Vector2> OnInputStart = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnInputUpdate = new UnityEvent<Vector2>();
    public UnityEvent OnInputEnd = new UnityEvent();

    private void Update()
    {
        switch (inputType)
        {
            case InputType.Mouse:
                HandleMouseInput();
                break;
            case InputType.Keyboard:
                HandleKeyboardInput();
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            OnInputStart?.Invoke(MousePosition);
        else if (Input.GetMouseButton(0))
            OnInputUpdate?.Invoke(MousePosition);
        else if (Input.GetMouseButtonUp(0))
            OnInputEnd?.Invoke();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(startKey))
            OnInputStart?.Invoke(GetTargetPosition());
        else if (Input.GetKey(updateKey))
            OnInputUpdate?.Invoke(GetTargetPosition());
        else if (Input.GetKeyUp(endKey))
            OnInputEnd?.Invoke();
    }

    private Vector2 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private Vector2 GetTargetPosition()
    {
        if (targetObject == null)
            targetObject = GameObject.FindGameObjectWithTag("Player").transform;

        if (targetObject != null)
            return targetObject.position;

        return Vector2.zero;
    }
}