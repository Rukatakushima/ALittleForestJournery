using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public UnityEvent<Vector2> OnMouseDown;
    public UnityEvent<Vector2> OnMouseDrag;
    public UnityEvent OnMouseUp;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown?.Invoke(GetMousePosition());
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseDrag?.Invoke(GetMousePosition());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp?.Invoke();
        }
    }

    private Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}