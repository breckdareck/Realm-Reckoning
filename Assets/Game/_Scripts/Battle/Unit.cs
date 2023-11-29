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
        [field: SerializeField] public UI_Unit UIUnit { get; private set; }
        [ShowInInspector] public UnitDataSO UnitsDataSo { get; private set; }

        // Public Battle Variables
        [ShowInInspector] public float TurnProgress { get; private set; }
        [ShowInInspector] public bool IsTakingTurn { get; private set; }
        [ShowInInspector] public bool IsAIUnit { get; private set; }
        [ShowInInspector] public int CurrentHealth { get; private set; }
        [ShowInInspector] public int MaxHealth => (int)CurrentBattleStats[GeneralStat.Health];
        [ShowInInspector] public int CurrentBarrier { get; private set; }
        [ShowInInspector] public int MaxBarrier { get; private set; }

        // This is a way of Tracking the Units Stats without messing with the Core UnitData Stats.
        [ShowInInspector] public Dictionary<GeneralStat, float> CurrentBattleStats { get; private set; }

        // This allows StatusEffects to modify other Stats during the Battle.
        [ShowInInspector] public Dictionary<GeneralStat, float> BattleBonusStats { get; private set; }

        [ShowInInspector] public List<StatusEffectSO> StatusEffects { get; private set; } = new();

        private readonly int _maxBarrierPercent = 10;

        private void Awake()
        {
            if (UIUnit == null)
                UIUnit = GetComponent<UI_Unit>();
        }


        public void Initialize(UnitDataSO dataSo, bool isAIUnit)
        {
            IsAIUnit = isAIUnit;
            UnitsDataSo = dataSo;
            SetupCurrentUnitBattleStats();
            //MaxHealth = (int)data.persistentStats[GeneralStat.Health];
            MaxBarrier = MaxHealth * _maxBarrierPercent;
            CurrentHealth = MaxHealth;
            BattleBonusStats = new Dictionary<GeneralStat, float>();
            var unit = this;
            UIUnit.InitializeUI(ref unit);
        }

        public void UpdateTurnProgress(float deltaTime)
        {
            if (IsTakingTurn) return;

            TurnProgress += UnitsDataSo.baseStatsSo.GetStatValue(GeneralStat.Speed) * deltaTime;

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

        private void SetupCurrentUnitBattleStats()
        {
            //UnitsData.SetupPersistentStats();
            CurrentBattleStats = new Dictionary<GeneralStat, float>(UnitsDataSo.persistentDataSo.stats);
        }

        public void ApplyDamage(int damageAmount)
        {
            if (damageAmount == 0)
            {
                UIUnit.CreateDamageText("Dodged");
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

            if (damageRemaining > 0) CurrentHealth -= Mathf.Clamp(damageRemaining, 0, MaxHealth);

            var combinedDamage = damageRemaining + oldBarrierAmount;

            UIUnit.CreateDamageText(combinedDamage.ToString());
            UIUnit.UpdateHealthUI();
            UIUnit.UpdateBarrierUI();
            Debug.Log($"{name} - Damage Taken: {combinedDamage}");
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
            if (StatusEffects.Exists(x => x.StatusEffectName == "Buff Immunity")) return;

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
                        UnitsDataSo.persistentDataSo.stats[data.StatEffected] + (float)data.EffectAmountPercent / 100 *
                        effect.StackCount;
                else
                    BattleBonusStats[data.StatEffected] =
                        UnitsDataSo.persistentDataSo.stats[data.StatEffected] * ((float)data.EffectAmountPercent /
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