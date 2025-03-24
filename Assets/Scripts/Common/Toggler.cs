using UnityEngine;

public class Toggler : RequireTarget<GameObject>
{
    [SerializeField] private bool isActive = false;

    private void Start() => SetActive(isActive);

    public void Toggle() => SetActive(!isActive);

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        target.SetActive(isActive);
    }
}