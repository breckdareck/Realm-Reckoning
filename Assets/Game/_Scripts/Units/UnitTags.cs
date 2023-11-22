using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Units
{
    [CreateAssetMenu(fileName = "New Unit Tags", menuName = "Custom/Unit Tags")]
    [ManageableData]
    public class UnitTags : ScriptableObject
    {
        public string unitTag;
    }
}