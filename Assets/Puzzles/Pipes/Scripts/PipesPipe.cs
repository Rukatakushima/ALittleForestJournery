using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pipes
{
    public class Pipe : MonoBehaviour
    {
        [SerializeField] private Transform[] pipePrefabs;
        
        [HideInInspector] public bool isFilled;
        [HideInInspector] public int pipeType;
        
        private Transform _currentPipe;
        private int _rotation;
        private SpriteRenderer _emptySprite, _filledSprite;
        private List<Transform> _connectBoxes;
        
        private const int MIN_ROTATION = 0;
        private const int MAX_ROTATION = 3;
        private const int ROTATION_MULTIPLIER = 90;

        public void Init(int pipe)
        {
            pipeType = pipe % 10;
            _currentPipe = Instantiate(pipePrefabs[pipeType], transform);
            _currentPipe.transform.localPosition = Vector3.zero;

            if (pipeType is 1 or 2) _rotation = pipe / 10;
            else _rotation = Random.Range(MIN_ROTATION, MAX_ROTATION + 1);

            _currentPipe.transform.eulerAngles = new Vector3(0, 0, _rotation * ROTATION_MULTIPLIER);

            if (pipeType is 0 or 1) isFilled = true;

            if (pipeType == 0) return;

            _emptySprite = _currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
            _emptySprite.gameObject.SetActive(!isFilled);
            _filledSprite = _currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
            _filledSprite.gameObject.SetActive(isFilled);

            _connectBoxes = new List<Transform>();
            for (int i = 2; i < _currentPipe.childCount; i++)
            {
                _connectBoxes.Add(_currentPipe.GetChild(i));
            }
        }

        public void RotatePipe()
        {
            if (pipeType is 0 or 1 or 2) return;

            _rotation = (_rotation + 1) % (MAX_ROTATION + 1);
            _currentPipe.transform.eulerAngles = new Vector3(0, 0, _rotation * ROTATION_MULTIPLIER);
        }

        public void UpdateFillState()
        {
            if (pipeType == 0) return;
            _emptySprite.gameObject.SetActive(!isFilled);
            _filledSprite.gameObject.SetActive(isFilled);
        }

        public List<Pipe> ConnectedPipes()
        {
            List<Pipe> result = new List<Pipe>();

            foreach (var hit in _connectBoxes.Select(box =>
                         Physics2D.RaycastAll(box.transform.position, Vector2.zero, 0.1f)))
            {
                result.AddRange(hit.Select(t => t.collider.transform.parent.parent.GetComponent<Pipe>()));
            }

            return result;
        }
    }
}