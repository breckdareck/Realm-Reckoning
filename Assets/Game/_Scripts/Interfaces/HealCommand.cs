using Game._Scripts.Battle;

namespace Game._Scripts.Interfaces
{
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
}