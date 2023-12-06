using Game._Scripts.Battle;
using Game._Scripts.Enums;
using UnityEngine;

namespace Game._Scripts.Interfaces
{
    public class AttackCommand : ICommand
    {
        private readonly int _damageAmountPercent;
        private readonly bool _canPierceBarrier;
        private readonly DamageType _damageType;

        public AttackCommand(int damageAmountPercent, bool canPierceBarrier, DamageType damageType)
        {
            _damageAmountPercent = damageAmountPercent;
            _canPierceBarrier = canPierceBarrier;
            _damageType = damageType;
        }

        public void Execute(BattleUnit source, BattleUnit target)
        {
            var damage = 0;
            var chanceToHit = CalculateHitChance(source, target, _damageType);
            
            var hitLanded = Random.Range(0f, 100f) <= chanceToHit;

            if (!hitLanded)
            {
                damage = 0;
                target.ApplyDamage(damage, true);
                return;
            }

            damage = CalculateDamage(source);
            
            var chanceToCrit = CalculateCritChance(source, target, _damageType);
            var isCrit = Random.Range(0f, 100f) <= chanceToCrit;
            if (isCrit)
                damage = (int)(damage * (source.CurrentBattleStats[GeneralStat.CriticalDamage] / 100f));

            
            damage -= CalculateArmorReduction(source, target, damage);

            var sourceDN = source.CurrentBattleStats[GeneralStat.DefenseNegation];
            var guranteedDamage = (int)(damage * (sourceDN / 100));
            damage += guranteedDamage;

            // TODO: Implement Barrier Piercing
            // TODO - Implement Health Steal

            target.ApplyDamage(Mathf.Clamp(damage, 0, 99999), false);
        }
        
        
        private float CalculateHitChance(BattleUnit source, BattleUnit target, DamageType damageType) 
        {
            var sourceStat = damageType == DamageType.PhysicalDamage 
                ? source.CurrentBattleStats[GeneralStat.PhysicalAccuracy] 
                : source.CurrentBattleStats[GeneralStat.MagikAccuracy];
            
            var targetStat = damageType == DamageType.PhysicalDamage 
                ? target.CurrentBattleStats[GeneralStat.PhysicalDodge] 
                : target.CurrentBattleStats[GeneralStat.MagikDodge];

            return 100 - Mathf.Clamp(targetStat - sourceStat, 0, 100);
        }
        
        private int CalculateDamage(BattleUnit source) 
        {
            var offenseStat = _damageType == DamageType.PhysicalDamage 
                ? source.CurrentBattleStats[GeneralStat.PhysicalOffense] 
                : source.CurrentBattleStats[GeneralStat.MagikOffense];

            return (int)((float)_damageAmountPercent / 100 * offenseStat);
        }
        
        private float CalculateCritChance(BattleUnit source, BattleUnit target, DamageType damageType)
        {
            var sourceStat = damageType == DamageType.PhysicalDamage 
                ? source.CurrentBattleStats[GeneralStat.PhysicalCriticalChance] 
                : source.CurrentBattleStats[GeneralStat.MagikCriticalChance];

            var targetStat = damageType == DamageType.PhysicalDamage 
                ? target.CurrentBattleStats[GeneralStat.PhysicalCriticalAvoidance] 
                : target.CurrentBattleStats[GeneralStat.MagikCriticalAvoidance];

            return Mathf.Clamp(sourceStat - targetStat, 0, 100);
        }

        private int CalculateArmorReduction(BattleUnit source, BattleUnit target, int damage)
        {
            var sourceStat = _damageType == DamageType.PhysicalDamage 
                ? source.CurrentBattleStats[GeneralStat.ArmorPierce] 
                : source.CurrentBattleStats[GeneralStat.MagikArmorPierce];

            var targetStat = _damageType == DamageType.PhysicalDamage 
                ? target.CurrentBattleStats[GeneralStat.Armor] 
                : target.CurrentBattleStats[GeneralStat.MagikArmor];

            var remainingArmorPercent = Mathf.Clamp(targetStat - sourceStat, 0, 100) / 100;
            return (int)(damage * remainingArmorPercent);
        }
        
    }
}