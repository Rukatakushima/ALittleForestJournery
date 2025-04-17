using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorBoolController : RequireTarget<Animator>
{
    [SerializeField] private string boolParameterName;
    [SerializeField] public bool IsActive { get; private set; } = false;

    public void Toggle() => SetActive(!IsActive);

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        target.SetBool(boolParameterName, isActive);
    }
}