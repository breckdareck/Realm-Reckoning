using Game._Scripts.Battle;
using Game._Scripts.Enums;
using UnityEngine;

namespace Game._Scripts.Interfaces
{
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
            var sourcePA = source.UnitsDataSo.persistentDataSo.stats[GeneralStat.PhysicalAccuracy];
            var targetPD = target.UnitsDataSo.persistentDataSo.stats[GeneralStat.PhysicalDodge];
            var chanceToHit = 100 - Mathf.Clamp(targetPD - sourcePA, 0, 100);
            var hitLanded = Random.Range(0f, 100f) <= chanceToHit;

            if (!hitLanded)
            {
                damage = 0;
                target.ApplyDamage(damage);
                return;
            }

            damage = (int)((float)_damageAmount / 100 *
                           source.UnitsDataSo.persistentDataSo.stats[GeneralStat.PhysicalOffense]);
            // TODO - Implement Magik Attacks

            var sourcePCC = source.UnitsDataSo.persistentDataSo.stats[GeneralStat.PhysicalCriticalChance];
            var targetPCA = target.UnitsDataSo.persistentDataSo.stats[GeneralStat.PhysicalCriticalAvoidance];
            var chanceToCrit = Mathf.Clamp(sourcePCC - targetPCA, 0, 100);
            var isCrit = Random.Range(0f, 100f) <= chanceToCrit;

            if (isCrit)
                damage = (int)(damage * source.UnitsDataSo.persistentDataSo.stats[GeneralStat.CriticalDamage] / 100);

            var sourceAP = source.UnitsDataSo.persistentDataSo.stats[GeneralStat.ArmorPierce];
            var targetArmor = target.UnitsDataSo.persistentDataSo.stats[GeneralStat.Armor];
            var remainingArmorPercent = Mathf.Clamp(targetArmor - sourceAP, 0, 100) / 100;

            var sourceDN = source.UnitsDataSo.persistentDataSo.stats[GeneralStat.DefenseNegation];
            var guranteedDamage = (int)(damage * (sourceDN / 100));

            damage -= (int)(damage * remainingArmorPercent);

            damage += guranteedDamage;

            // TODO - Implement Health Steal

            target.ApplyDamage(Mathf.Clamp(damage, 0, 99999));
        }
    }
}