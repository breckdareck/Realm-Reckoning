using System.Collections.Generic;
using Game._Scripts.Units;
using UnityEngine;

namespace Game._Scripts.Battle
{
    [CreateAssetMenu(fileName = "New Mission", menuName = "Custom/New Mission")]
    public class Mission : ScriptableObject
    {
        public string missionName;
        public List<UnitData> missionUnitDatas;
    }
}
