using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    [SerializeField] private float cameraSize = 2f;
    [SerializeField] private Vector3 cameraPosition = new Vector3(2f, 2f, -10f);

    public void SetupCamera()
    {
        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = cameraSize;
    }

    public void SetupCamera(Vector4 cameraSettings)
    {
        Camera.main.transform.position = cameraSettings;
        Camera.main.orthographicSize = cameraSettings.w;
    }
}
