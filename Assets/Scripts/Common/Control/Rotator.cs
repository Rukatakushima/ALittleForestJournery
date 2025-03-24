using UnityEngine;

public class Rotator : RequireTarget<Transform>
{
    [SerializeField] protected float rotationSpeed = 90f;
    [SerializeField] protected Vector3 rotationDirection = Vector3.forward;

    public void Rotate() => target.Rotate(rotationDirection, rotationSpeed * Time.deltaTime);
}