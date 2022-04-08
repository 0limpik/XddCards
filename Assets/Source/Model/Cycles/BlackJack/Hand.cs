using Assets.Source.Model.Cash;
using Assets.Source.Model.Cycles.BlackJack.Controllers;
using Assets.Source.Model.Games.BlackJack.Users;

namespace Assets.Source.Model.Cycles.BlackJack
{
    public class Hand
    {
        public IPlayer User;

        public decimal Amount => bet == null ? 0 : doubleBet == null ? bet.Amount : bet.Amount + doubleBet.Amount;

        internal Bet bet;
        internal Bet doubleBet;
        internal bool HasBet => bet != null && bet.Amount > 0;

        internal GameController gameController;

        public void Hit()
        {
            gameController.Hit(User);
        }

        public void Stand()
        {
            gameController.Stand(User);
        }

        public void DoubleUp()
        {
            gameController.DoubleUp(User);
        }
    }
}
