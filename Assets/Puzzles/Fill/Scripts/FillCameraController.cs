using UnityEngine;

namespace Fill
{
    public class CameraController : DefaultCameraController
    {
        public void SetupCamera()
        {
            if (Camera.main == null) return;
            
            float x = GameManager.Instance.Level.columns;
            float y = GameManager.Instance.Level.rows;

            Camera.main.transform.position = new Vector3(x * cameraPositionController, y * cameraPositionController, cameraPositionZ);
            Camera.main.orthographicSize = ((x + y) / 2) * cameraSizeController;
        }
    }
}