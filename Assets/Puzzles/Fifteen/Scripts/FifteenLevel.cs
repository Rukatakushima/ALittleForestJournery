using UnityEngine;

namespace Fifteen
{
    [CreateAssetMenu(fileName = "FifteenLevel", menuName = "Levels/FifteenLevel")]
    public class LevelData : ScriptableObject
    {
        public int shuffleTimes = 6;
    }
}