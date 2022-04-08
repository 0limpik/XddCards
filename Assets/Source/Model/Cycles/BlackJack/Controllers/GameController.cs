using System;
using System.Linq;
using Assets.Source.Model.Cash;
using Assets.Source.Model.Games;
using Assets.Source.Model.Games.BlackJack;
using Assets.Source.Model.Games.BlackJack.Users;

namespace Assets.Source.Model.Cycles.BlackJack.Controllers
{
    public class GameController : State
    {
        private const string c_playerCount = "Players must be more 0";

        private IBlackJack game = new Game();

        public event Action OnGameEnd
        {
            add => game.OnGameEnd += value;
            remove => game.OnGameEnd -= value;
        }

        public event Action<ICard> OnDillerUpHiddenCard
        {
            add => game.OnDillerUpHiddenCard += value;
            remove => game.OnDillerUpHiddenCard -= value;
        }

        public Hand dealerHand { get; private set; }

        private User[] players;

        private IPlayer[] Players => game.players;

        internal GameController(User[] players)
        {
            this.players = players;

            dealerHand = new Hand { User = game.dealer };
        }

        public void Start()
        {
            game.Start();
        }

        public void Hit(IPlayer player)
        {
            CheckExecute();
            game.Hit(player);
        }

        public void Stand(IPlayer player)
        {
            CheckExecute();
            game.Stand(player);
        }

        public void DoubleUp(IPlayer player)
        {
            CheckExecute();
            foreach (var play in players)
            {
                foreach (var hand in play.hands)
                {
                    if (hand.User == player)
                    {
                        if (play.wallet.CanReserve(hand.bet.Amount))
                        {
                            hand.doubleBet = play.wallet.Reserve(hand.bet.Amount);
                        }
                        else
                        {
                            throw new InvalidOperationException("bet greater cash");
                        }
                        game.Hit(player);
                        if (player.CanTurn)
                            game.Stand(player);
                        return;
                    }
                }
            }
            throw new Exception("hand not found");
        }

        protected override void Enter()
        {
            game.Init(players
            .SelectMany(x => x.hands)
            .Where(x => x.HasBet)
            .Count());

            var playerCount = 0;

            foreach (var player in players)
            {
                foreach (var hand in player.hands)
                {
                    var user = game.players[playerCount++];
                    hand.User = user;

                    user.OnResult += (result) => OnResult(result, player, hand);
                }
            }
        }

        protected override void Exit()
        {

        }

        private void OnResult(GameResult result, User player, Hand hand)
        {
            var bet = hand.bet;
            var doubleBet = hand.doubleBet;
            hand.bet = hand.doubleBet = null;

            Handle(bet);

            if (doubleBet != null)
                Handle(doubleBet);

            void Handle(Bet bet)
            {
                if (result == GameResult.Win)
                {
                    player.wallet.Give(bet);
                    return;
                }
                if (result == GameResult.Lose)
                {
                    player.wallet.Take(bet);
                    return;
                }
                if (result == GameResult.Push)
                {
                    player.wallet.Cancel(bet);
                    return;
                }
                throw new Exception($"uninspected {nameof(GameResult)}");
            }
        }

        public override bool CanEnter(out string message)
        {
            message = null;
            return true;
        }

        public override bool CanExit(out string message)
        {
            message = null;
            return true;
        }
    }
}
