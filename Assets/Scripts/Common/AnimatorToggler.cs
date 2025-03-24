using UnityEngine;

public class AnimatorToggler : RequireTarget<Animator>
{
    [SerializeField] private string boolParameterName;
    [SerializeField] private bool isActive = false;

    public void Toggle() => SetActive(!isActive);

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        target.SetBool(boolParameterName, isActive);
    }
}