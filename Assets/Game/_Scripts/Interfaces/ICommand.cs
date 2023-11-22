using Game._Scripts.Units;

namespace Game._Scripts.Interfaces
{
    public interface ICommand
    {
        void Execute(Unit source, Unit target);
    }
    
    public class AttackCommand : ICommand
    {
        private readonly int _damageAmount;

        public AttackCommand(int damageAmount)
        {
            _damageAmount = damageAmount;
        }

        public void Execute(Unit source, Unit target)
        {
            target.TakeDamage(_damageAmount);
        }
    }

    public class BuffCommand : ICommand
    {

        private readonly int _buffAmount;
        private readonly Stat[] _stats;

        public BuffCommand(int buffAmount, params Stat[] stats)
        {
            _buffAmount = buffAmount;
            _stats = stats;
        }
        
        public void Execute(Unit source, Unit target)
        {
            foreach (var stat in _stats)
            {
                // You might want to add some validation or checks here
                target.ApplyBuff(stat, _buffAmount);
            }
        }
    }
    
    public class DebuffCommand : ICommand
    {
        private readonly int _debuffAmount;
        private readonly Stat[] _affectedStats;

        public DebuffCommand(int debuffAmount, params Stat[] affectedStats)
        {
            _debuffAmount = debuffAmount;
            _affectedStats = affectedStats;
        }

        public void Execute(Unit source, Unit target)
        {
            // Implement the logic to debuff the specified stats
            foreach (var stat in _affectedStats)
            {
                // You might want to add some validation or checks here
                target.ApplyDebuff(stat, _debuffAmount);
            }
        }
    }

}