using System.Collections.Generic;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New MissionDatabase", menuName = "Custom/New MissionDatabase")]
    public class MissionDatabaseSO : ScriptableObject
    {
        public List<MissionSO> missions;

        public MissionSO GetMissionByName(string missionName)
        {
            return missions.Find(x => x.missionName == missionName);
        }
    }
}