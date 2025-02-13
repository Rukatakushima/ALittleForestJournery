using UnityEngine;

namespace OneStroke
{
    public class Edge : MonoBehaviour
    {
        public bool isFilled { get; private set; }

        [Header("Line Settings")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Gradient startGradient;
        [SerializeField] private Gradient activeGradient;

        [Header("Line Renderer Parameters")]
        [SerializeField] private int positionCount = 2;
        [SerializeField] private int startPositionIndex = 0;
        [SerializeField] private int endPositionIndex = 1;

        public void Initialize(Vector2 start, Vector2 end)
        {
            lineRenderer.positionCount = positionCount;
            SetLinePositions(start, end);
            SetGradient(startGradient);
            isFilled = false;
        }

        private void SetLinePositions(Vector2 start, Vector2 end)
        {
            lineRenderer.SetPosition(startPositionIndex, start);
            lineRenderer.SetPosition(endPositionIndex, end);
        }

        private void SetGradient(Gradient gradient)
        {
            lineRenderer.colorGradient = gradient;
        }

        public void Add()
        {
            isFilled = true;
            SetGradient(activeGradient);
        }
    }
}