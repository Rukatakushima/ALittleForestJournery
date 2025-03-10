using UnityEngine;

public class DefaultCameraController : MonoBehaviour
{
    [SerializeField] protected float cameraSizeController = 2f;
    [SerializeField] protected float cameraPositionController = 2f;
    [SerializeField] protected float cameraPositionZ = -10f;

    public virtual void SetupCamera(float levelSize)
    {
        Camera.main.transform.position = new Vector3(levelSize * cameraPositionController, levelSize * cameraPositionController, cameraPositionZ);
        Camera.main.orthographicSize = levelSize * cameraSizeController;
    }
}