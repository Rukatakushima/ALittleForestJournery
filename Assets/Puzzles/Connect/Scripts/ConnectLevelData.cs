using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    [CreateAssetMenu(fileName = "ConnectLevel", menuName = "Levels/ConnectLevel")]
    public class LevelData : ScriptableObject
    {
        public List<Edge> connections;
    }

    [System.Serializable]
    public struct Edge
    {
        public List<Vector2Int> points;
        // если в списке уже есть точки, то возвращаем самую первую(как старт)
        public Vector2Int StartPoint => points is { Count: > 0 } ? points[0] : new Vector2Int(-1, -1);
        // если в списке уже есть точки, то возвращаем поселднюю(как конец)
        public Vector2Int EndPoint => points is { Count: > 0 } ? points[^1] : new Vector2Int(-1, -1);
    }
}
