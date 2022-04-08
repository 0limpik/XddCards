using Xdd.Model.Cash;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Model.Games.BlackJack.Users;

namespace Xdd.Model.Cycles.BlackJack
{
    public class Hand
    {
        public IPlayer player;

        public decimal Amount => bet == null ? 0 : doubleBet == null ? bet.Amount : bet.Amount + doubleBet.Amount;

        internal Bet bet;
        internal Bet doubleBet;
        internal bool HasBet => bet != null && bet.Amount > 0;

        internal GameController gameController;

        public void Hit()
        {
            gameController.Hit(player);
        }

        public void Stand()
        {
            gameController.Stand(player);
        }

        public void DoubleUp()
        {
            gameController.DoubleUp(player);
        }
    }
}
