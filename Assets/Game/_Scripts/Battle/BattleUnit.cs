using System;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Game._Scripts.UI.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Battle
{
    [Serializable]
    public class BattleUnit : MonoBehaviour
    {
        private const int MaxBarrierPercent = 10;
        
        [field: SerializeField] public UI_BattleUnit UIBattleUnit { get; private set; }
        [ShowInInspector] public Unit Unit { get; private set; }

        // Public Battle Variables
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public float TurnProgress { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public bool IsTakingTurn { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public bool IsControlledByAI { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public bool IsDead => CurrentHealth <= 0;

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int CurrentHealth { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int MaxHealth => (int)CurrentBattleStats[GeneralStat.Health];

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int CurrentBarrier { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int MaxBarrier { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public Dictionary<GeneralStat, float> CurrentBattleStats { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public Dictionary<GeneralStat, float> BattleBonusStats { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public List<StatusEffectSO> StatusEffects { get; private set; } = new();
        
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public Dictionary<AbilitySO, int> AbilityCooldowns { get; private set; } = new ();


        private void Awake()
        {
            if (UIBattleUnit == null)
                UIBattleUnit = GetComponent<UI_BattleUnit>();
        }

        public void Initialize(Unit unit, bool isAIUnit)
        {
            IsControlledByAI = isAIUnit;
            Unit = unit;
            CurrentBattleStats = new Dictionary<GeneralStat, float>(Unit.currentUnitStats);
            //MaxHealth = (int)CurrentBattleStats[GeneralStat.Health];
            MaxBarrier = MaxHealth * MaxBarrierPercent;
            CurrentHealth = MaxHealth;
            BattleBonusStats = new Dictionary<GeneralStat, float>();
            var battleUnit = this;
            UIBattleUnit.InitializeUI(ref battleUnit);
        }

        public void UpdateTurnProgress(float deltaTime)
        {
            if (IsTakingTurn) return;

            TurnProgress += CurrentBattleStats[GeneralStat.Speed] * deltaTime;

            UIBattleUnit.UpdateTurnSliderValue(TurnProgress);

            if (!(TurnProgress >= 1000f)) return;
            Debug.Log($"{name}'s turn!");
            StartTurn();
        }

        public void StartTurn()
        {
            IsTakingTurn = true;
        }

        public void EndTurn()
        {
            IsTakingTurn = false;
            TurnProgress %= 1000f;
            TickDownStatusEffects();
            List<AbilitySO> keys = new List<AbilitySO>(AbilityCooldowns.Keys);
            foreach(AbilitySO ability in keys)
            { 
                AbilityCooldowns[ability]--;  
                if(AbilityCooldowns[ability]==0) 
                    AbilityCooldowns.Remove(ability);
            }
        }

        public void ApplyDamage(int damageAmount, bool isAttackDodged)
        {
            if (isAttackDodged)
            {
                UIBattleUnit.CreateDamageText("Dodged");
                return;
            }

            if (damageAmount == 0)
            {
                UIBattleUnit.CreateDamageText("Damage Negated");
                return;
            }

            // TODO : If Unit has Status Effect - Unassailable, all damage is null. Only calculate Debuffs
            // TODO : Change Color of damage text depending on Damage Type
            // TODO : Add Barrier and Pierce Barrier to formula
            // TODO : Calculate Barrier reduction formula to incoming damage
            
            var resultDamage = ComputeDamage(damageAmount);
            
            UIBattleUnit.CreateDamageText(resultDamage.ToString());
            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
            Debug.Log($"{name} - Damage Taken: {resultDamage}");
            if (!IsDead) return;
            Debug.Log($"{name} - Is Dead");
            gameObject.SetActive(false);
        }
        
        private int ComputeDamage(int damageAmount)
        {
            var damageRemaining = damageAmount;
            var oldBarrierAmount = ApplyDamageToBarrier(ref damageRemaining);
            ApplyDamageToHealth(ref damageRemaining);
            return damageRemaining + oldBarrierAmount;
        }
        
        private int ApplyDamageToBarrier(ref int remainingDamage)
        {
            var oldBarrierAmount = 0;
            if (CurrentBarrier > 0)
            {
                if (remainingDamage > CurrentBarrier)
                {
                    remainingDamage -= CurrentBarrier;
                    oldBarrierAmount = CurrentBarrier;
                    CurrentBarrier = 0;
                }
                else
                {
                    UIBattleUnit.CreateDamageText(remainingDamage.ToString());
                    CurrentBarrier -= Mathf.Clamp(remainingDamage, 0, MaxBarrier);
                    remainingDamage = 0;
                }
            }
            return oldBarrierAmount;
        }
        
        private void ApplyDamageToHealth(ref int remainingDamage)
        {
            if (remainingDamage > 0) 
            {
                CurrentHealth = Mathf.Clamp(CurrentHealth - remainingDamage, 0, MaxHealth);
            }
        }
        
        public void ApplyHeal(int healAmount, int barrierAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, MaxHealth);
            CurrentBarrier = Mathf.Clamp(CurrentBarrier + barrierAmount, 0, MaxBarrier);
            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
            UIBattleUnit.CreateHealText(healAmount);
            Debug.Log($"{name} - Heal Received: {healAmount}");
        }

        public void ApplyStatusEffect(StatusEffectSO statusEffectSo)
        {
            // Check For Stat (Buff Immunity)
            if (StatusEffects.Exists(x => x.StatusEffectName == "Buff Immunity") &&
                statusEffectSo.StatusEffectType == StatusEffectType.Buff) return;

            // Check For Stacking, Max Stacks
            if (statusEffectSo.CanStack &&
                StatusEffects.Exists(x => x.StatusEffectName == statusEffectSo.StatusEffectName))
            {
                var effect = StatusEffects.Find(x => x.StatusEffectName == statusEffectSo.StatusEffectName);
                effect.IncreaseStackCount();
                effect.ResetTurnsEffected();
            }
            else if (!statusEffectSo.CanStack &&
                     StatusEffects.Exists(x => x.StatusEffectName == statusEffectSo.StatusEffectName))
            {
                StatusEffects.Find(x => x.StatusEffectName == statusEffectSo.StatusEffectName).ResetTurnsEffected();
            }
            else
            {
                statusEffectSo.ResetTurnsEffected();
                StatusEffects.Add(statusEffectSo);
            }

            RemoveCurrentBonusStats();
            CalculateBattleBonusStats();
            RecalculateCurrentStats();
        }

        private void RemoveCurrentBonusStats()
        {
            foreach (var stat in BattleBonusStats)
            {
                CurrentBattleStats[stat.Key] -= stat.Value;
                if (stat.Key == GeneralStat.Health) CurrentHealth -= (int)stat.Value;
            }

            BattleBonusStats.Clear();
        }

        private void CalculateBattleBonusStats()
        {
            foreach (var effect in StatusEffects)
            foreach (var data in effect.StatusEffectDatas)
                if (effect.StatusEffectCalculationType == StatusEffectCalculationType.Additive)
                    BattleBonusStats[data.StatEffected] =
                        (float)data.EffectAmountPercent / 100 * effect.StackCount;
                else
                    BattleBonusStats[data.StatEffected] =
                        Unit.currentUnitStats[data.StatEffected] * ((float)data.EffectAmountPercent /
                                                                           100 *
                                                                           effect.StackCount);
        }

        private void RecalculateCurrentStats()
        {
            foreach (var stat in BattleBonusStats)
            {
                CurrentBattleStats[stat.Key] += stat.Value;
                if (stat.Key == GeneralStat.Health)
                    CurrentHealth = Mathf.Clamp(CurrentHealth + (int)stat.Value, 0, MaxHealth);
            }

            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
        }

        public void TickDownStatusEffects()
        {
            if (StatusEffects.Count > 0)
            {
                foreach (var statusEffect in StatusEffects)
                {
                    if (statusEffect.AppliedThisTurn)
                    {
                        statusEffect.SetAppliedThisTurn(false);
                        continue;
                    }

                    statusEffect.TickDownStatusEffect();
                }

                StatusEffects.RemoveAll(x => x.RemainingTurnsEffected <= 0);
                RemoveCurrentBonusStats();
                CalculateBattleBonusStats();
                RecalculateCurrentStats();
            }
        }
        
        public bool IsAbilityOnCooldown(AbilitySO stuff)
        {
            if(AbilityCooldowns.ContainsKey(stuff))
            {
                return AbilityCooldowns[stuff] > 0;
            }
            return false;
        }
        
        public void StartCooldown(AbilitySO ability)
        {
            int cooldown = ability.cooldownTurns + 1;
            if(AbilityCooldowns.ContainsKey(ability))
                AbilityCooldowns[ability] = cooldown;
            else
                AbilityCooldowns.Add(ability, cooldown);
        }
    }
}