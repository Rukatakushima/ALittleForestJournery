using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMiniMap : MonoBehaviour
{

    public Animator showMiniMap;
    public bool mapIsShown = false;
    public void ShowMap()
    {
        if (mapIsShown) 
        {
            showMiniMap.SetBool("mapOpen", false);
            mapIsShown = false;
        }
        else 
        {
            showMiniMap.SetBool("mapOpen", true);  
            mapIsShown = true;  
        }   
    }
}
