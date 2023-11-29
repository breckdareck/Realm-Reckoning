using System.Collections.Generic;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Mission", menuName = "Custom/New Mission")]
    public class MissionSO : ScriptableObject
    {
        public string missionName;
        public List<UnitDataSO> missionUnitDatas;
    }
}