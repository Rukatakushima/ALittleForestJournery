using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    [CreateAssetMenu(fileName = "OneStrokeLevel", menuName = "Levels/OneStrokeLevel")]
    public class LevelData : ScriptableObject
    {
        public int Rows, Columns;
        public List<Vector3> Points;
        public List<Vector2Int> Edges;
    }
}