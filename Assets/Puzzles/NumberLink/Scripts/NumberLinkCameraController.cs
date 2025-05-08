using UnityEngine;

namespace NumberLink
{
    public class CameraController : DefaultCameraController
    {
        public void SetupCamera(float width, Vector3 backgroundPosition)
        {
            if (Camera.main == null) return;
            Camera.main.orthographicSize = width * cameraSizeController;
            Camera.main.transform.position = new Vector3(backgroundPosition.x, backgroundPosition.y, cameraPositionZ);
        }
    }
}