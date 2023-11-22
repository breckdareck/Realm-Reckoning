using System.Collections.Generic;
using UnityEngine;

namespace Game._Scripts.Battle
{
    [CreateAssetMenu(fileName = "New MissionDatabase", menuName = "Custom/New MissionDatabase")]
    public class MissionDatabase : ScriptableObject
    {
        public List<Mission> missions;

        public Mission GetMissionByName(string missionName)
        {
            return missions.Find(x => x.missionName == missionName);
        }
    }
    
    
}
