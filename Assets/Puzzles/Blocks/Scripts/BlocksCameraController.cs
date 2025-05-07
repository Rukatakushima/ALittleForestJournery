using UnityEngine;

namespace Blocks
{
    public class CameraController : DefaultCameraController
    {
        [SerializeField] private float backgroundCellPositionRate = 0.5f;
        [SerializeField] private float rowsOffset = 2f;

        public void SetupCamera()
        {
            if (Camera.main == null) return;
            
            LevelData level = GameManager.Instance.Level;
            float blockSpawnSize = GameManager.Instance.DefaultBlockSize;

            float maxColumns = Mathf.Max(level.gridColumns, level.blockColumns * blockSpawnSize);
            float maxRows = level.gridRows + rowsOffset + level.blockRows * blockSpawnSize;

            Camera.main.orthographicSize = Mathf.Max(maxColumns, maxRows) * cameraSizeController;
            Camera.main.transform.position = new Vector3(level.gridColumns * backgroundCellPositionRate,
                (level.gridRows + backgroundCellPositionRate) * backgroundCellPositionRate, cameraPositionZ);
        }
    }
}