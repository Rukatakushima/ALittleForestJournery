using System.Collections.Generic;
using UnityEngine;

namespace Connect.Common
{
    [CreateAssetMenu(fileName = "ConnectLevel", menuName = "Levels/Connect/ConnectAllLevel")]
    public class ConnectLevelList : ScriptableObject
    {
        public List<ConnectLevelData> Levels;
    }
}

