using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FillLevel", menuName = "FillLevel")]
public class Level : ScriptableObject
{
    public int Row;
    public int Col;
    public List<int> Data;
}
