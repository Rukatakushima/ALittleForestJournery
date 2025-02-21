using System.Collections.Generic;
using UnityEngine;

namespace NumberLink
{
    [CreateAssetMenu(fileName = "NumberLinkLevel", menuName = "Levels/NumberLinkLevel")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] public int Rows, Columns;
        [SerializeField] public List<int> Data;
    }
}