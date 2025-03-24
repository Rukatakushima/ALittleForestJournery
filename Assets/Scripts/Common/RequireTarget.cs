using UnityEngine;

public class RequireTarget<T> : MonoBehaviour where T : Object
{
    [SerializeField] protected T target;

    protected virtual void Awake()
    {
        if (target == null)
        {
            Debug.LogWarning($"Target objects are not assigned in {GetType().Name}.", this);
            enabled = false;
        }
    }
}