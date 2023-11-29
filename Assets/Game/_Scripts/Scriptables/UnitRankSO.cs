using Game._Scripts.Attributes;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Unit Rank", menuName = "Custom/Unit Rank")]
    [ManageableData]
    public class UnitRankSO : ScriptableObject
    {
        public string unitRank;
    }
}