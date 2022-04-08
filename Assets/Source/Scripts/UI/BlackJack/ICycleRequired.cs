using Assets.Source.Model.Cycles.BlackJack;

namespace Assets.Source.Scripts.UI.BlackJack
{
    internal interface ICycleRequired
    {
        void InitCycle(BJCycle cycle, User user);
    }
}
