using UnityEngine;

namespace NumberLink
{
    public class CameraController : DefaultCameraController
    {
        public void SetupCamera(float width, Vector3 backgroundPosition)
        {
            /*
            
            Vector3 bgPos = new Vector3(
                width / 2f - cellSize / 2f - levelGap / 2f,
                height / 2f - cellSize / 2f - levelGap / 2f,
                0
                );
                
            float width = (cellSize + cellGap) * levelData.col - cellGap + levelGap;
            */
            Camera.main.orthographicSize = width * cameraSizeController;
            Camera.main.transform.position = new Vector3(backgroundPosition.x, backgroundPosition.y, cameraPositionZ);
        }
    }
}