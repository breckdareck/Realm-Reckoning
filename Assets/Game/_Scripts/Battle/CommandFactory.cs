using System.Linq;
using Game._Scripts.Abilities;
using Game._Scripts.Enums;
using Game._Scripts.Interfaces;
using UnityEngine;

namespace Game._Scripts.Battle
{
    public static  class CommandFactory
    {
        public static ICommand CreateCommand(AbilityAction action)
        {
            switch (action.actionType)
            {
                case ActionType.Attack:
                    return new AttackCommand(action.damagePercent, false, action.damageType);

                case ActionType.Heal:
                    return new HealCommand(action.healAmount, action.barrierAmount);

                case ActionType.StatusEffect:
                    var newStatusEffects = action.statusEffect.Select(effect => Object.Instantiate(effect)).ToList();
                    return new StatusEffectCommand(newStatusEffects);

                default:
                    Debug.LogWarning("Unsupported action type");
                    return null;
            }
        }
    }
}