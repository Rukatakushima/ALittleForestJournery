using UnityEngine;

namespace NumberLink
{
    public class CameraController : BaseCameraController
    {
        public void SetupCamera(float width, Vector3 bgPos)
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
            Camera.main.transform.position = new Vector3(bgPos.x, bgPos.y, cameraPositionZ);
        }
    }
}