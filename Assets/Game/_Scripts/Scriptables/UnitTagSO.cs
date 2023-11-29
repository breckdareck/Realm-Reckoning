using Game._Scripts.Attributes;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Unit Tag", menuName = "Custom/Unit Tag")]
    [ManageableData]
    public class UnitTagSO : ScriptableObject
    {
        public string unitTag;
    }
}