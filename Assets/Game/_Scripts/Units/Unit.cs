using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Units
{
    [Serializable]
    public class Unit : MonoBehaviour
    {
        [field: SerializeField] public UnitUI UnitUI { get; private set; }
        [ShowInInspector] public UnitData UnitsData { get; private set; }
        
        // Public Battle Variables
        public float TurnProgress { get; private set; }
        public bool IsTakingTurn { get; private set; }
        public bool IsAIUnit { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int CurrentBarrier { get; private set; }
        public int MaxBarrier { get; private set; }

        private readonly int _maxBarrierPercent = 10;


        public void Initialize(UnitData data, bool isAIUnit)
        {
            IsAIUnit = isAIUnit;
            UnitsData = data;
            SetupCurrentUnitDataStats();
            MaxHealth = (int)data.currentStats.GetStatValue(GeneralStat.Health);
            MaxBarrier = MaxHealth * _maxBarrierPercent;
            CurrentHealth = MaxHealth;
            var unit = this;
            UnitUI.InitializeUI(ref unit);
        }

        public void UpdateTurnProgress(float deltaTime)
        {
            if (IsTakingTurn) return;
            
            TurnProgress += UnitsData.baseStats.GetStatValue(GeneralStat.Speed) * deltaTime;

            UnitUI.UpdateTurnSliderValue(TurnProgress);

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
        }

        private void SetupCurrentUnitDataStats()
        {
            // Create a new Instance of Stats for the current stats of the unit based on the formulas
            UnitsData.currentStats = ScriptableObject.CreateInstance<Stats>();

            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(GeneralStat.Strength);
            var agi = SetupAttributeValue(GeneralStat.Agility);
            var mag = SetupAttributeValue(GeneralStat.Magik);
            
            // Setup Str, Agi, Mag
            UnitsData.currentStats.generalStats.Add(GeneralStat.Strength, str);
            UnitsData.currentStats.generalStats.Add(GeneralStat.Agility, agi);
            UnitsData.currentStats.generalStats.Add(GeneralStat.Magik, mag);
            
            // Setup Physical and Magikal Offense
            UnitsData.currentStats.generalStats.Add(GeneralStat.PhysicalOffense, (str * 2.5f + agi));
            UnitsData.currentStats.generalStats.Add(GeneralStat.MagikOffense, (mag * 2.5f + agi));
            
            // Setup Potency and Resilience
            UnitsData.currentStats.generalStats.Add(GeneralStat.Potency, UnitsData.baseStats.GetStatValue(GeneralStat.Potency));
            UnitsData.currentStats.generalStats.Add(GeneralStat.Resilience, UnitsData.baseStats.GetStatValue(GeneralStat.Resilience));
            
            // Setup Physical Accuracy and Dodge
            UnitsData.currentStats.generalStats.Add(GeneralStat.PhysicalAccuracy, UnitsData.baseStats.GetStatValue(GeneralStat.PhysicalAccuracy));
            UnitsData.currentStats.generalStats.Add(GeneralStat.PhysicalDodge, UnitsData.baseStats.GetStatValue(GeneralStat.PhysicalDodge));
            
            // Setup Magikal Accuracy and Dodge
            UnitsData.currentStats.generalStats.Add(GeneralStat.MagikAccuracy, UnitsData.baseStats.GetStatValue(GeneralStat.MagikAccuracy));
            UnitsData.currentStats.generalStats.Add(GeneralStat.MagikDodge, UnitsData.baseStats.GetStatValue(GeneralStat.MagikDodge));
            
            // Setup CritDmg and Defense Negation
            UnitsData.currentStats.generalStats.Add(GeneralStat.CriticalDamage, UnitsData.baseStats.GetStatValue(GeneralStat.CriticalDamage));
            UnitsData.currentStats.generalStats.Add(GeneralStat.DefenseNegation, UnitsData.baseStats.GetStatValue(GeneralStat.DefenseNegation));
            
            // Setup PhysicalCritChance, PhysicalCritAvoidance
            UnitsData.currentStats.generalStats.Add(GeneralStat.PhysicalCriticalChance, UnitsData.baseStats.GetStatValue(GeneralStat.PhysicalCriticalChance));
            UnitsData.currentStats.generalStats.Add(GeneralStat.PhysicalCriticalAvoidance, UnitsData.baseStats.GetStatValue(GeneralStat.PhysicalCriticalAvoidance));
            
            // Setup Armor Pierce and Magik Armor Pierce
            UnitsData.currentStats.generalStats.Add(GeneralStat.ArmorPierce, UnitsData.baseStats.GetStatValue(GeneralStat.ArmorPierce));
            UnitsData.currentStats.generalStats.Add(GeneralStat.MagikArmorPierce, UnitsData.baseStats.GetStatValue(GeneralStat.MagikArmorPierce));
            
            // Setup Armor and MagArmor
            UnitsData.currentStats.generalStats.Add(GeneralStat.Armor, UnitsData.baseStats.GetStatValue(GeneralStat.Armor));
            UnitsData.currentStats.generalStats.Add(GeneralStat.MagikArmor, UnitsData.baseStats.GetStatValue(GeneralStat.MagikArmor));
            
            // TODO : Add Gear Additives to Health Formula
            // Formula = (Str*35.6)+(Agi*11.6)+(Mag*17.5) + Gear additives
            var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            UnitsData.currentStats.generalStats.Add(GeneralStat.Health, health);
            
            // TODO : Add Gear Additives to Speed Formula
            UnitsData.currentStats.generalStats.Add(GeneralStat.Speed, (int)UnitsData.baseStats.GetStatValue(GeneralStat.Speed));
        }

        private float SetupAttributeValue(GeneralStat stat)
        {
            switch (stat)
            {
                case GeneralStat.Strength :
                    return (UnitsData.baseStats.GetStatValue(GeneralStat.Strength) +
                            (UnitsData.baseStats.GetStatValue(LevelUpBonus.StrengthPerLevel) *
                             (UnitsData.baseStats.GetStatValue(GeneralStat.Level) - 1)) *
                            Mathf.Pow(1.22f,UnitsData.baseStats.GetStatValue(GeneralStat.StarRating)));
                case GeneralStat.Agility:
                    return (UnitsData.baseStats.GetStatValue(GeneralStat.Agility) +
                     (UnitsData.baseStats.GetStatValue(LevelUpBonus.AgilityPerLevel) *
                      (UnitsData.baseStats.GetStatValue(GeneralStat.Level) - 1)) *
                     Mathf.Pow(1.22f,UnitsData.baseStats.GetStatValue(GeneralStat.StarRating)));
                case GeneralStat.Magik:
                    return (UnitsData.baseStats.GetStatValue(GeneralStat.Magik) +
                            (UnitsData.baseStats.GetStatValue(LevelUpBonus.MagikPerLevel) *
                             (UnitsData.baseStats.GetStatValue(GeneralStat.Level) - 1)) *
                            Mathf.Pow(1.22f,UnitsData.baseStats.GetStatValue(GeneralStat.StarRating)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
            
        }

        public void ApplyDamage(int damageAmount)
        {
            if (damageAmount == 0)
            {
                UnitUI.CreateDamageText("Dodged");
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
                    UnitUI.CreateDamageText(damageRemaining.ToString());
                    CurrentBarrier -= Mathf.Clamp(damageRemaining, 0, MaxBarrier);
                    damageRemaining = 0;
                }
            }
            
            if (damageRemaining > 0)
            {
                CurrentHealth -= Mathf.Clamp(damageRemaining, 0, MaxHealth);
            }
            
            var combinedDamage = damageRemaining + oldBarrierAmount;
            
            UnitUI.CreateDamageText(combinedDamage.ToString());
            UnitUI.UpdateHealthUI();
            UnitUI.UpdateBarrierUI();
            Debug.Log($"{name} - Damage Taken: {combinedDamage}");
        }
        
        public void ApplyHeal(int healAmount, int barrierAmount)
        {
            CurrentHealth += Mathf.Clamp(healAmount, 0, MaxHealth);
            CurrentBarrier += Mathf.Clamp(barrierAmount, 0, MaxBarrier);
            UnitUI.UpdateHealthUI();
            UnitUI.UpdateBarrierUI();
            UnitUI.CreateHealText(healAmount);
            Debug.Log($"{name} - Heal Received: {healAmount}");
        }

        public void ApplyBuff(GeneralStat generalStat, int buffAmount)
        {
            Debug.Log($"{generalStat} was buffed by {buffAmount} points");
        }

        public void ApplyDebuff(GeneralStat generalStat, int debuffAmount)
        {
            Debug.Log($"{generalStat} was debuffed by {debuffAmount} points");
        }

        
    }
}