using UnityEngine;

namespace OneStroke
{
    public class Edge : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Gradient emptyGradient, filledGradient, wrongGradient;

        private const int POS_COUNT = 2;
        private const int START_POS_ID = 0;
        private const int END_POS_ID = 1;

        public bool IsFilled { get; private set; }
        
        public void SetEmptyEdge(Vector2 startPosition, Vector2 endPosition)
        {
            lineRenderer.positionCount = POS_COUNT;
            SetLinePositions(startPosition, endPosition);
            SetGradient(emptyGradient);
            IsFilled = false;
        }

        private void SetLinePositions(Vector2 start, Vector2 end)
        {
            lineRenderer.SetPosition(START_POS_ID, start);
            lineRenderer.SetPosition(END_POS_ID, end);
        }

        public void SetStartHighlight(Vector2 startPosition)
        {
            gameObject.SetActive(true);
            lineRenderer.positionCount = POS_COUNT;
            UpdateHighlightPosition(startPosition);
        }

        public void UpdateHighlightPosition(Vector2 startPosition)
        {
            lineRenderer.SetPosition(START_POS_ID, startPosition);
            lineRenderer.SetPosition(END_POS_ID, startPosition);
        }

        public void SetEndPosition(Vector2 endPosition) => lineRenderer.SetPosition(END_POS_ID, endPosition);

        public void TurnOff() => gameObject.SetActive(false);

        private void SetGradient(Gradient gradient) => lineRenderer.colorGradient = gradient;

        public void FillEdge()
        {
            IsFilled = true;
            SetFilledGradient();
        }

        public void SetWrongGradient() => SetGradient(wrongGradient);

        public void SetFilledGradient() => SetGradient(filledGradient);
    }
}