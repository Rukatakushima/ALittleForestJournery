using System.Collections.Generic;
using UnityEngine;

namespace OneStroke
{
    [CreateAssetMenu(fileName = "OneStrokeLevel", menuName = "Levels/OneStrokeLevel")]
    public class LevelData : ScriptableObject
    {
        public List<Vector3> points;
        public List<Vector2Int> edges;
    }
}