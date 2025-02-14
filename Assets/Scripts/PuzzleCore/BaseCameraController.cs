using UnityEngine;

public abstract class BaseCameraController : MonoBehaviour
{
    [SerializeField] protected float cameraPositionController = 0.5f;
    [SerializeField] protected float cameraPositionZ = -10f;
    [SerializeField] protected float cameraSizeController = 2f;

    public virtual void SetupCamera(float levelSize)
    {
        // Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);
        // Camera.main.orthographicSize = currentLevelSize / cameraSizeController;

        // Vector3 cameraPosition = Camera.main.transform.position;
        // cameraPosition.x = level.Columns / cameraPositionController;
        // cameraPosition.y = level.Rows / cameraPositionController;
        // Camera.main.transform.position = cameraPosition;
        // Camera.main.orthographicSize = Mathf.Max(level.Columns, level.Rows) * cameraSizeController;

        Camera.main.transform.position = new Vector3(levelSize / cameraPositionController, levelSize / cameraPositionController, cameraPositionZ);

        Camera.main.orthographicSize = levelSize * cameraSizeController;
    }
}
