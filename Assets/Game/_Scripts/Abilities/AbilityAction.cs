﻿using System;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Sirenix.OdinInspector;

namespace Game._Scripts.Abilities
{
    [Serializable]
    public class AbilityAction
    {
        public ActionType actionType;
        public TargetType targetType;
        public TargetSelection targetSelection;

        [ShowIf("IsAttackAction")] public DamageType damageType;
        [ShowIf("IsAttackAction")] public int damagePercent;

        [ShowIf("IsHealAction")] public int healAmount;
        [ShowIf("IsHealAction")] public int barrierAmount;

        [ShowIf("IsStatusEffectAction")] public List<StatusEffectSO> statusEffect;

        private bool IsAttackAction => actionType == ActionType.Attack;
        private bool IsHealAction => actionType == ActionType.Heal;
        private bool IsStatusEffectAction => actionType == ActionType.StatusEffect;
    }
}