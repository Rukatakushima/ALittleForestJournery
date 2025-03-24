using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    [SerializeField] public int ID;
    // public Sprite Icon { get; protected set; }

    protected virtual void Show() => gameObject.SetActive(true);
    protected virtual void Hide() => gameObject.SetActive(false);

    public abstract void Spawn();
}