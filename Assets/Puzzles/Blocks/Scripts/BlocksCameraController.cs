using UnityEngine;

namespace Blocks
{
    public class CameraController : BaseCameraController
    {
        public float backgroundCellPositionRate = 0.5f;

        public void SetupCamera(LevelData level, float blockSpawnSize, float blockRows, float blockColumns)
        {
            float maxColumns = Mathf.Max(level.Columns, blockColumns * blockSpawnSize);
            float maxRows = level.Rows + 2f + blockRows * blockSpawnSize;

            Camera.main.orthographicSize = Mathf.Max(maxColumns, maxRows) * cameraSizeController;
            Camera.main.transform.position = new Vector3(level.Columns * backgroundCellPositionRate, (level.Rows + backgroundCellPositionRate) * backgroundCellPositionRate, cameraPositionZ);
        }
    }
}