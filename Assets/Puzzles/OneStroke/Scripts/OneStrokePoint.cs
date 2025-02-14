using UnityEngine;

namespace OneStroke
{
    public class Point : MonoBehaviour
    {
        public int Id { get; private set; }
        public Vector2 Position { get; private set; }

        public void SetPoint(int id, Vector2 position)
        {
            Id = id;
            Position = position;
            transform.position = Position;
            name = id.ToString();
        }
    }
}