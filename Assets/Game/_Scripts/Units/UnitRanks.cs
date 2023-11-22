using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Units
{
    [CreateAssetMenu(fileName = "New Unit Ranks", menuName = "Custom/Unit Ranks")]
    [ManageableData]
    public class UnitRanks : ScriptableObject
    {
        public string unitRank;
    }
}