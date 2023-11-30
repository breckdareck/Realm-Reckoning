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

        public void Execute(Unit source, Unit target)
        {
            var damage = 0;
            var chanceToHit = 100f;

            if (_damageType == DamageType.PhysicalDamage)
            {
                var sourcePA = source.CurrentBattleStats[GeneralStat.PhysicalAccuracy];
                var targetPD = target.CurrentBattleStats[GeneralStat.PhysicalDodge];
                chanceToHit = 100 - Mathf.Clamp(targetPD - sourcePA, 0, 100);
            }
            else
            {
                var sourceMA = source.CurrentBattleStats[GeneralStat.MagikAccuracy];
                var targetMD = target.CurrentBattleStats[GeneralStat.MagikDodge];
                chanceToHit = 100 - Mathf.Clamp(targetMD - sourceMA, 0, 100);
            }
            
            var hitLanded = Random.Range(0f, 100f) <= chanceToHit;

            if (!hitLanded)
            {
                damage = 0;
                target.ApplyDamage(damage,true);
                return;
            }

            var chanceToCrit = 0f;

            if (_damageType == DamageType.PhysicalDamage)
            {
                damage = (int)((float)_damageAmountPercent / 100 *
                               source.CurrentBattleStats[GeneralStat.PhysicalOffense]);
                var sourcePCC = source.CurrentBattleStats[GeneralStat.PhysicalCriticalChance];
                var targetPCA = target.CurrentBattleStats[GeneralStat.PhysicalCriticalAvoidance];
                chanceToCrit = Mathf.Clamp(sourcePCC - targetPCA, 0, 100);
            }
            else
            {
                damage = (int)((float)_damageAmountPercent / 100 *
                               source.CurrentBattleStats[GeneralStat.MagikOffense]);
                var sourceMCC = source.CurrentBattleStats[GeneralStat.MagikCriticalChance];
                var targetMCA = target.CurrentBattleStats[GeneralStat.MagikCriticalAvoidance];
                chanceToCrit = Mathf.Clamp(sourceMCC - targetMCA, 0, 100);
            }
            
            var isCrit = Random.Range(0f, 100f) <= chanceToCrit;

            if (isCrit)
                damage = (int)(damage * (source.CurrentBattleStats[GeneralStat.CriticalDamage] / 100f));

            
            if (_damageType == DamageType.PhysicalDamage)
            {
                var sourceAP = source.CurrentBattleStats[GeneralStat.ArmorPierce];
                var targetArmor = target.CurrentBattleStats[GeneralStat.Armor];
                var remainingArmorPercent = Mathf.Clamp(targetArmor - sourceAP, 0, 100) / 100;
                
                damage -= (int)(damage * remainingArmorPercent);
            }
            else
            {
                var sourceMP = source.CurrentBattleStats[GeneralStat.MagikArmorPierce];
                var targetMagikArmor = target.CurrentBattleStats[GeneralStat.MagikArmor];
                var remainingMagikArmorPercent = Mathf.Clamp(targetMagikArmor - sourceMP, 0, 100) / 100;
                
                damage -= (int)(damage * remainingMagikArmorPercent);
            }

            var sourceDN = source.CurrentBattleStats[GeneralStat.DefenseNegation];
            var guranteedDamage = (int)(damage * (sourceDN / 100));
            
            damage += guranteedDamage;
            
            // TODO: Implement Barrier Piercing
            // TODO - Implement Health Steal

            target.ApplyDamage(Mathf.Clamp(damage, 0, 99999),false);
        }
    }
}