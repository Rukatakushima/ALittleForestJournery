using UnityEngine;

namespace Fill
{
    public class CameraController : DefaultCameraController
    {
        public void SetupCamera(/*float x, float y*/)
        {
            float x = GameManager.Instance.level.Columns;
            float y = GameManager.Instance.level.Rows;

            Camera.main.transform.position = new Vector3(x * cameraPositionController, y * cameraPositionController, cameraPositionZ);
            Camera.main.orthographicSize = ((x + y) / 2) * cameraSizeController;
        }
    }
}