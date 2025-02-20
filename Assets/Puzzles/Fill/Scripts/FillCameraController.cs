using UnityEngine;

namespace Fill
{
    public class CameraController : BaseCameraController
    {
        public void SetupCamera(float x, float y)
        {
            Camera.main.transform.position = new Vector3(x * cameraPositionController, y * cameraPositionController, cameraPositionZ);
            Camera.main.orthographicSize = ((x + y) / 2) * cameraSizeController;
        }
    }
}