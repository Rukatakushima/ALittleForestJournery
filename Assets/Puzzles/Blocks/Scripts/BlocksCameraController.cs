using UnityEngine;

namespace Blocks
{
    public class CameraController : DefaultCameraController
    {
        [SerializeField] public float backgroundCellPositionRate { get; private set; } = 0.5f;
        [SerializeField] private float rowsOffset = 2f;

        public void SetupCamera()
        {
            LevelData level = GameManager.Instance.level;
            float blockSpawnSize = GameManager.Instance.defaultBlockSize;

            float maxColumns = Mathf.Max(level.GridColumns, level.BlockColumns * blockSpawnSize);
            float maxRows = level.GridRows + rowsOffset + level.BlockRows * blockSpawnSize;

            Camera.main.orthographicSize = Mathf.Max(maxColumns, maxRows) * cameraSizeController;
            Camera.main.transform.position = new Vector3(level.GridColumns * backgroundCellPositionRate, (level.GridRows + backgroundCellPositionRate) * backgroundCellPositionRate, cameraPositionZ);
        }
    }
}