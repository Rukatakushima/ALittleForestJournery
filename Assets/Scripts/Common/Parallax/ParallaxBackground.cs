using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
  public ParallaxCamera parallaxCamera;
  private readonly List<ParallaxLayer> _parallaxLayers = new List<ParallaxLayer>();

  private void Awake()
  {
      parallaxCamera ??= Camera.main?.GetComponent<ParallaxCamera>();
  }

  private void Start()
  {
    parallaxCamera?.onCameraTranslate.AddListener(Move);
    SetLayers();
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
    foreach (ParallaxLayer layer in _parallaxLayers)
    {
      layer.Move(delta);
    }
  }
}