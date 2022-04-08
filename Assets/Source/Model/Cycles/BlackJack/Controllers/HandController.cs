using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Source.Model.Cycles.BlackJack.Controllers
{
    public class HandController : State
    {
        private const string c_handCount = "Hand need more 0";

        private User[] players;
        private List<Hand> hands;

        public Hand GetHand => hands.First();

        private int HandCount => players.SelectMany(x => x.hands).Count();

        internal HandController(User[] players, List<Hand> hands)
        {
            this.players = players;
            this.hands = hands;
        }

        internal void Take(User player)
        {
            Check(player);

            var hand = hands.FirstOrDefault();

            if (hand == null)
                throw new InvalidOperationException("has't free hand");

            hands.Remove(hand);

            player.hands.Add(hand);
        }

        internal void Release(User player)
        {
            Check(player);

            var hand = player.hands.FirstOrDefault();

            if (hand == null)
                throw new ArgumentException("has't hands");

            player.hands.Remove(hand);

            hands.Add(hand);
        }

        void Check(User player)
        {
            CheckExecute();

            if (!players.Contains(player))
                throw new ArgumentException();
        }

        protected override void Enter()
        {

        }

        protected override void Exit()
        {
            if (HandCount <= 0)
                throw new InvalidOperationException(c_handCount);
        }

        public override bool CanEnter(out string message)
        {
            message = null;
            return true;
        }

        public override bool CanExit(out string message)
        {
            if (HandCount > 0)
            {
                message = null;
                return true;
            }
            else
            {
                message = c_handCount;
                return false;
            }
        }
    }
}
