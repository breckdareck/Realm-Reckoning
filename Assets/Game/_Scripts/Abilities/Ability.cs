using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Custom/Ability")]
    public class Ability : ScriptableObject
    {
        [FormerlySerializedAs("AbilityName")] public string abilityName;
        public AbilityAction[] actions;
    }


    [System.Serializable]
    public class AbilityAction
    {
        public ActionType actionType;
        public TargetType targetType;
        public TargetSelection targetSelection;

        [ShowIf("IsAttackAction")] public int damageAmount;
        
        [ShowIf("IsHealAction")] public int healAmount;
        [ShowIf("IsHealAction")] public int barrierAmount;
        
        [ShowIf("IsBuffAction")] public BuffType buffType;
        [ShowIf("IsBuffAction")] public int buffAmount;
        
        [ShowIf("IsDebuffAction")] public DebuffType debuffType;
        [ShowIf("IsDebuffAction")] public int debuffAmount;

        private bool IsAttackAction => actionType == ActionType.Attack;
        private bool IsHealAction => actionType == ActionType.Heal;
        private bool IsBuffAction => actionType == ActionType.Buff;
        private bool IsDebuffAction => actionType == ActionType.Debuff;
    }

    public enum TargetSelection
    {
        Manual, // Player manually selects the target
        Auto // Automatically select the target based on some logic
        // Add more options as needed
    }

    public enum ActionType
    {
        Attack,
        Heal,
        Buff,
        Debuff
        // Add more action types
    }

    public enum TargetType
    {
        Enemy,
        Ally
        // Add more target types
    }

    // Additional enums for Buff/Debuff types
    public enum BuffType
    {
        IncreaseAttack,
        IncreaseDefense
        // Add more buff types
    }

    public enum DebuffType
    {
        DecreaseAttack,
        DecreaseDefense
        // Add more debuff types
    }
}