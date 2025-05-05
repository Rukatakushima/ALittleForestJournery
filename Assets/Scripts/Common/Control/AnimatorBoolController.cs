using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorBoolController : RequireTarget<Animator>
{
    [SerializeField] private string boolParameterName;
    private bool _isActive;

    public void Toggle() => SetActive(!_isActive);

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        target.SetBool(boolParameterName, isActive);
    }
}