using System;
using Game._Scripts.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game._Scripts.Interfaces
{
    public interface ICommand
    {
        void Execute(Unit source, Unit target);
    }

    public class AttackCommand : ICommand
    {
        private readonly int _damageAmount;
        private readonly bool _canPierceBarrier;

        public AttackCommand(int damageAmount, bool canPierceBarrier)
        {
            _damageAmount = damageAmount;
            _canPierceBarrier = canPierceBarrier;
        }

        public void Execute(Unit source, Unit target)
        {
            var damage = 0;
            
            // TODO: Implement Barrier Piercing
            var sourcePA = source.UnitsData.currentStats.GetStatValue(GeneralStat.PhysicalAccuracy);
            var targetPD = target.UnitsData.currentStats.GetStatValue(GeneralStat.PhysicalDodge);
            var chanceToHit = 100 - Mathf.Clamp(targetPD - sourcePA, 0, 100);
            var hitLanded = (Random.Range(0f, 100f) <= chanceToHit);
            
            if (!hitLanded)
            {
                damage = 0;
                target.ApplyDamage(damage);
                return;
            }

            damage = (int)(_damageAmount * source.UnitsData.currentStats.GetStatValue(GeneralStat.PhysicalOffense));
            // TODO - Implement Magik Attacks
            // TODO - Implement Defense Negation
            
            var sourcePCC = source.UnitsData.currentStats.GetStatValue(GeneralStat.PhysicalCriticalChance);
            var targetPCA = target.UnitsData.currentStats.GetStatValue(GeneralStat.PhysicalCriticalAvoidance);
            var chanceToCrit = Mathf.Clamp(sourcePCC - targetPCA, 0, 100);
            var isCrit = (Random.Range(0f, 100f) <= chanceToCrit);

            if(isCrit)
                damage = (int)(damage * source.UnitsData.currentStats.GetStatValue(GeneralStat.CriticalDamage)/100);

            var sourceAP = source.UnitsData.currentStats.GetStatValue(GeneralStat.ArmorPierce);
            var targetArmor = target.UnitsData.currentStats.GetStatValue(GeneralStat.Armor);
            var remainingArmor = Mathf.Clamp(targetArmor - sourceAP, 0, 100);

            var sourceDN = source.UnitsData.currentStats.GetStatValue(GeneralStat.DefenseNegation);

            remainingArmor = (remainingArmor / 85) - (remainingArmor * (sourceDN / 100));
            
            damage -= (int)remainingArmor;
            
            // TODO - Implement Health Steal
            
            target.ApplyDamage(damage);
        }
    }

    public class HealCommand : ICommand
    {
        private readonly int _healAmount;
        private readonly int _barrierAmount;

        public HealCommand(int healAmount, int barrierAmount)
        {
            _healAmount = healAmount;
            _barrierAmount = barrierAmount;
        }

        public void Execute(Unit source, Unit target)
        {
            target.ApplyHeal(_healAmount, _barrierAmount);
        }
    }

    public class BuffCommand : ICommand
    {
        private readonly int _buffAmount;
        private readonly GeneralStat[] _stats;

        public BuffCommand(int buffAmount, params GeneralStat[] stats)
        {
            _buffAmount = buffAmount;
            _stats = stats;
        }

        public void Execute(Unit source, Unit target)
        {
            foreach (var stat in _stats)
                // You might want to add some validation or checks here
                target.ApplyBuff(stat, _buffAmount);
        }
    }

    public class DebuffCommand : ICommand
    {
        private readonly int _debuffAmount;
        private readonly GeneralStat[] _affectedStats;

        public DebuffCommand(int debuffAmount, params GeneralStat[] affectedStats)
        {
            _debuffAmount = debuffAmount;
            _affectedStats = affectedStats;
        }

        public void Execute(Unit source, Unit target)
        {
            // Implement the logic to debuff the specified stats
            foreach (var stat in _affectedStats)
            {
                var sourcePOT = source.UnitsData.currentStats.GetStatValue(GeneralStat.Potency);
                var targetRES = target.UnitsData.currentStats.GetStatValue(GeneralStat.Resilience);
                var chanceToHit = 100 - Mathf.Clamp(targetRES - sourcePOT, 0, 100);

                var hitLanded = (Random.Range(0f, 100f) <= chanceToHit) ;
                if(!hitLanded) continue;
                
                target.ApplyDebuff(stat, _debuffAmount);
            }
        }
    }
}