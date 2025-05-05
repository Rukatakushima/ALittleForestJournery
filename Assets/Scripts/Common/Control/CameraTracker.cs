using UnityEngine;
using UnityEngine.Events;

public class CameraMovementTracker : MonoBehaviour
{
	public UnityEvent<float> onCameraMovedOnX;
	private float _oldPositionX;

	private void Start() => _oldPositionX = transform.position.x;

	private void Update()
	{
		if (Mathf.Approximately(transform.position.x, _oldPositionX)) return;
		
		if (onCameraMovedOnX != null)
		{
			var delta = _oldPositionX - transform.position.x;
			onCameraMovedOnX?.Invoke(delta);
		}
		_oldPositionX = transform.position.x;
	}
}