using UnityEngine;

public abstract class BaseCameraController : MonoBehaviour
{
    protected float cameraPositionController = 0.5f;
    protected float cameraSizeController = 2f;

    public virtual void SetupCamera(BaseLevelData level)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = level.Columns / cameraPositionController;
        cameraPosition.y = level.Rows / cameraPositionController;
        Camera.main.transform.position = cameraPosition;

        Camera.main.orthographicSize = Mathf.Max(level.Columns, level.Rows) * cameraSizeController;
    }
}
