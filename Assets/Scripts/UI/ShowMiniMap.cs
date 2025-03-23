using UnityEngine;

public class ShowMiniMap : MonoBehaviour
{
    public Animator showMiniMap;
    private bool mapIsShown = false;

    public void ToggleMap()
    {
        mapIsShown = !mapIsShown;
        showMiniMap.SetBool("mapOpen", mapIsShown);
    }
}