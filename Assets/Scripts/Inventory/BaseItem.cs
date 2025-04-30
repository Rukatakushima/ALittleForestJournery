using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    [field: SerializeField] public string ID { get; protected set; }

    public virtual void Show() => gameObject.SetActive(true);
    public virtual void Hide() => gameObject.SetActive(false);
    public abstract void Spawn();
}