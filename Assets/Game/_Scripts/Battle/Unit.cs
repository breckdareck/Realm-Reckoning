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
    public class Unit : MonoBehaviour
    {
        private const int MaxBarrierPercent = 10;

        
        [field: SerializeField] public UI_Unit UIUnit { get; private set; }
        [ShowInInspector] public UnitDataSO UnitsDataSo { get; private set; }

        // Public Battle Variables
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public float TurnProgress { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public bool IsTakingTurn { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public bool IsAIUnit { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public bool IsDead => CurrentHealth <= 0;
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public int CurrentHealth { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public int MaxHealth => (int)CurrentBattleStats[GeneralStat.Health];
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public int CurrentBarrier { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public int MaxBarrier { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public Dictionary<GeneralStat, float> CurrentBattleStats { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public Dictionary<GeneralStat, float> BattleBonusStats { get; private set; }
        [ShowInInspector, ReadOnly, FoldoutGroup("Battle Vars")] public List<StatusEffectSO> StatusEffects { get; private set; } = new();


        private void Awake()
        {
            if (UIUnit == null)
                UIUnit = GetComponent<UI_Unit>();
        }
        
        public void Initialize(UnitDataSO dataSo, bool isAIUnit)
        {
            IsAIUnit = isAIUnit;
            UnitsDataSo = dataSo;
            CurrentBattleStats = new Dictionary<GeneralStat, float>(UnitsDataSo.currentUnitStats);
            //MaxHealth = (int)CurrentBattleStats[GeneralStat.Health];
            MaxBarrier = MaxHealth * MaxBarrierPercent;
            CurrentHealth = MaxHealth;
            BattleBonusStats = new Dictionary<GeneralStat, float>();
            var unit = this;
            UIUnit.InitializeUI(ref unit);
        }

        public void UpdateTurnProgress(float deltaTime)
        {
            if (IsTakingTurn) return;

            TurnProgress += UnitsDataSo.baseUnitStats[GeneralStat.Speed] * deltaTime;

            UIUnit.UpdateTurnSliderValue(TurnProgress);

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
        }

        public void ApplyDamage(int damageAmount, bool isAttackDodged)
        {
            if (isAttackDodged)
            {
                UIUnit.CreateDamageText("Dodged");
                return;
            }

            if (damageAmount == 0)
            {
                UIUnit.CreateDamageText("Damage Negated");
                return;
            }

            // TODO : If Unit has Status Effect - Unassailable, all damage is null. Only calculate Debuffs

            // Set initial damage
            var damageRemaining = damageAmount;
            var oldBarrierAmount = 0;
            // TODO : Change Color of damage text depending on Damage Type
            // TODO : Add Barrier and Pierce Barrier to formula
            // Do damage to Barrier unless attack Pierces Barriers
            if (CurrentBarrier > 0)
            {
                // TODO : Calculate Barrier reduction formula to incoming damage
                if (damageRemaining > CurrentBarrier)
                {
                    damageRemaining = damageAmount - CurrentBarrier;
                    oldBarrierAmount = CurrentBarrier;
                    CurrentBarrier = 0;
                }
                else if (damageRemaining <= CurrentBarrier)
                {
                    UIUnit.CreateDamageText(damageRemaining.ToString());
                    CurrentBarrier -= Mathf.Clamp(damageRemaining, 0, MaxBarrier);
                    damageRemaining = 0;
                }
            }

            if (damageRemaining > 0) CurrentHealth = Mathf.Clamp(CurrentHealth - damageRemaining, 0, MaxHealth);

            var combinedDamage = damageRemaining + oldBarrierAmount;

            UIUnit.CreateDamageText(combinedDamage.ToString());
            UIUnit.UpdateHealthUI();
            UIUnit.UpdateBarrierUI();
            Debug.Log($"{name} - Damage Taken: {combinedDamage}");
            if (!IsDead) return;
            Debug.Log($"{name} - Is Dead");
            gameObject.SetActive(false);
        }

        public void ApplyHeal(int healAmount, int barrierAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, MaxHealth);
            CurrentBarrier = Mathf.Clamp(CurrentBarrier + barrierAmount, 0, MaxBarrier);
            UIUnit.UpdateHealthUI();
            UIUnit.UpdateBarrierUI();
            UIUnit.CreateHealText(healAmount);
            Debug.Log($"{name} - Heal Received: {healAmount}");
        }

        public void ApplyStatusEffect(StatusEffectSO statusEffectSo)
        {
            // Check For Stat (Buff Immunity)
            if (StatusEffects.Exists(x => x.StatusEffectName == "Buff Immunity") && statusEffectSo.StatusEffectType == StatusEffectType.Buff) return;

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
                        UnitsDataSo.currentUnitStats[data.StatEffected] * ((float)data.EffectAmountPercent /
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

            UIUnit.UpdateHealthUI();
            UIUnit.UpdateBarrierUI();
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
    }
}