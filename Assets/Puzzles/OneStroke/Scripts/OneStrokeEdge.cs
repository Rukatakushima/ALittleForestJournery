using UnityEngine;

namespace OneStroke
{
    public class Edge : MonoBehaviour
    {
        public bool IsFilled { get; private set; }

        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Gradient emptyGradient;
        [SerializeField] private Gradient filledGradient;
        [SerializeField] private Gradient wrongGradient;

        private const int positionCount = 2;
        private const int startPositionIndex = 0;
        private const int endPositionIndex = 1;

        public void SetEmptyEdge(Vector2 startPosition, Vector2 endPosition)
        {
            lineRenderer.positionCount = positionCount;
            SetLinePositions(startPosition, endPosition);
            SetGradient(emptyGradient);
            IsFilled = false;
            name = $"{startPosition} - {endPosition}";
        }

        private void SetLinePositions(Vector2 start, Vector2 end)
        {
            lineRenderer.SetPosition(startPositionIndex, start);
            lineRenderer.SetPosition(endPositionIndex, end);
        }

        public void SetStartHighlight(Vector2 startPosition)
        {
            gameObject.SetActive(true);
            lineRenderer.positionCount = positionCount;
            UpdateHighlightPosition(startPosition);
        }

        public void UpdateHighlightPosition(Vector2 startPosition)
        {
            lineRenderer.SetPosition(startPositionIndex, startPosition);
            lineRenderer.SetPosition(endPositionIndex, startPosition);
        }

        public void SetEndPosition(Vector2 endPosition) => lineRenderer.SetPosition(endPositionIndex, endPosition);

        public void TurnOff() => gameObject.SetActive(false);

        private void SetGradient(Gradient gradient) => lineRenderer.colorGradient = gradient;

        public void FillEdge()
        {
            IsFilled = true;
            SetFilledGradient();
        }

        public void SetWrongGradient() => SetGradient(wrongGradient);

        public void SetFilledGradient() => SetGradient(filledGradient);

        public void ResetEdge()
        {
            IsFilled = false;
            SetGradient(emptyGradient);
        }
    }
}