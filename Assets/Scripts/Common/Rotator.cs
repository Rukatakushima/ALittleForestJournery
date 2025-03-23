using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] protected float rotationSpeed = 90f;
    [SerializeField] protected Vector3 rotationDirection = Vector3.forward;

    public virtual void RotateClockwise() => transform.Rotate(rotationDirection, rotationSpeed * Time.deltaTime);
}