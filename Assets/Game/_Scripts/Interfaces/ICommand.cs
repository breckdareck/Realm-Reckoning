using System;
using Game._Scripts.Battle;

namespace Game._Scripts.Interfaces
{
    public interface ICommand
    {
        void Execute(BattleUnit source, BattleUnit target);
    }
}