using System.Collections.Generic;
using UnityEngine;

namespace Connect
{
    [CreateAssetMenu(fileName = "ConnectLevel", menuName = "Levels/ConnectLevel")]
    public class LevelData : ScriptableObject
    {
        public List<Edge> Connections;
    }

    [System.Serializable]
    public struct Edge
    {
        public List<Vector2Int> Points;
        public Vector2Int StartPoint
        {
            get
            {
                // если в списке уже есть точки, то возвращаем самую первую(как старт)
                if (Points != null && Points.Count > 0)
                {
                    return Points[0];
                }
                return new Vector2Int(-1, -1);
            }
        }
        public Vector2Int EndPoint
        {
            get
            {
                // если в списке уже есть точки, то возвращаем поселднюю(как конец)
                if (Points != null && Points.Count > 0)
                {
                    return Points[Points.Count - 1];
                }
                return new Vector2Int(-1, -1);
            }
        }
    }

}
