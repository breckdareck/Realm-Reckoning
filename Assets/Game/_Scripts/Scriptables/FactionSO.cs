using Game._Scripts.Attributes;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Faction", menuName = "Custom/Faction")]
    [ManageableData]
    public class FactionSO : ScriptableObject
    {
        public string factionName;
    }
}