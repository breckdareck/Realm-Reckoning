using Game._Scripts.Abilities;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Custom/Ability")]
    public class AbilitySO : ScriptableObject
    {
        public string abilityName;
        public AbilityAction[] actions;
        public int cooldownTurns;
    }
}