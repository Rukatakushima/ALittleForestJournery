using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
	private float _oldPosition;
	public UnityEvent<float> onCameraTranslate;

	private void Start() => _oldPosition = transform.position.x;

	private void Update()
	{
		if (Mathf.Approximately(transform.position.x, _oldPosition)) return;
		if (onCameraTranslate != null)
		{
			var delta = _oldPosition - transform.position.x;
			onCameraTranslate?.Invoke(delta);
		}
		_oldPosition = transform.position.x;
	}
}