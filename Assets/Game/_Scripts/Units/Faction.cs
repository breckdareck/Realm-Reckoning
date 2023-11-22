using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Units
{
    [CreateAssetMenu(fileName = "New Faction", menuName = "Custom/Faction")]
    [ManageableData]
    public class Faction : ScriptableObject
    {
        public string factionName;
    }
}