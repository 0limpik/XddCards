using System;
using System.Linq;

namespace Assets.Source.Model.Cycles.BlackJack.Controllers
{
    public class BetController : State
    {
        private const string c_playerHasTBets = "At least one player must have a bet";

        private User[] players;

        public decimal Amount => players.First().amount;
        public decimal HandCount => players.First().hands.Count;

        internal BetController(User[] players)
        {
            this.players = players;
        }

        internal bool CanBet(User player, decimal amount)
        {
            Check(player);

            return player.wallet.CanReserve(amount * player.hands.Count);
        }

        internal void Bet(User player, decimal amount)
        {
            Check(player);

            if (!player.wallet.CanReserve(amount * player.hands.Count))
                throw new ArgumentException("bet can't reserve");

            player.amount = amount;
        }

        protected override void Enter()
        {

        }

        protected override void Exit()
        {
            foreach (var player in players)
            {
                if (player.amount > 0)
                    foreach (var hand in player.hands)
                    {
                        hand.bet = player.wallet.Reserve(player.amount);
                    }

                player.amount = 0;
            }

            if (players.SelectMany(x => x.hands).All(x => !x.HasBet))
                throw new Exception(c_playerHasTBets);

        }

        public override bool CanEnter(out string message)
        {
            message = null;
            return true;
        }

        public override bool CanExit(out string message)
        {
            if (players.Any(x => x.amount > 0))
            {
                message = null;
                return true;
            }
            else
            {
                message = c_playerHasTBets;
                return false;
            }
        }

        private void Check(User player)
        {
            CheckExecute();

            if (!players.Contains(player))
                throw new ArgumentException();
        }
    }
}
