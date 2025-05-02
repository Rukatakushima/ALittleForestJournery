using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
  private readonly List<ParallaxLayer> _parallaxLayers = new();

  private void Awake()
  {
    SetLayers();

    if (Camera.main != null && Camera.main.TryGetComponent(out CameraMovementTracker parallaxCamera))
    {
      parallaxCamera.onCameraMovedOnX.AddListener(Move);
    }
  }

  private void SetLayers()
  {
    _parallaxLayers.Clear();

    for (int i = 0; i < transform.childCount; i++)
    {
      var layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

      if (layer != null)
      {
        _parallaxLayers.Add(layer);
      }
    }
  }

  private void Move(float delta)
  {
    foreach (var layer in _parallaxLayers)
    {
      layer.Move(delta);
    }
  }
}