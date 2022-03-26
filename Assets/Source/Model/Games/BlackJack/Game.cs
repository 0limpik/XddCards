﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Model.Games.BlackJack.Users;

namespace Assets.Source.Model.Games.BlackJack
{
    public class Game : IBlackJack
    {
        public event Action OnGameEnd;
        public event Action<Card> OnDillerUpHiddenCard;

        public IUser[] players => _players;
        public IUser dealer => _dealer;

        private Player[] _players;
        private Dealer _dealer;
        private Card dillerHiddenCard;

        public Deck deck = new Deck();

        public bool isGame { get; private set; }

        public Game()
        {
            _dealer = new Dealer(GameScores.GetBlackJackScores);
            _players = new Player[0];
        }

        public void Init(int playersCount)
        {
            if (playersCount <= 0)
                throw new ArgumentException("Players must be more 0");

            _players = new Player[playersCount];

            for (int i = 0; i < playersCount; i++)
            {
                _players[i] = new Player(GameScores.GetBlackJackScores, _dealer);
            }
        }

        public void Start()
        {
            isGame = true;

            _dealer.Reset();

            foreach (var player in _players)
            {
                player.Reset();
            }

            _dealer.AddCard(GetCard());
            dillerHiddenCard = GetCard();

            foreach (var player in _players)
            {
                player.AddCard(GetCard());
                player.AddCard(GetCard());
            }

            if (!_players.Any(x => x.CanTurn))
            {
                EndGame();
            }
        }

        public void Turn(IUser user, BlackJackTurn turn)
        {
            if (!isGame) throw new Exception("Game is End");

            var player = players.FirstOrDefault(x => x == user) as Player ?? throw new ArgumentException("Player not found");

            if (!player.CanTurn)
                throw new Exception("Can't turn");

            switch (turn)
            {
                case BlackJackTurn.Hit:
                    Hit(player);
                    break;
                case BlackJackTurn.Stand:
                    Stand(player);
                    break;
                case BlackJackTurn.Split:
                    break;
            }

            if (!_players.Any(x => x.CanTurn))
            {
                EndGame();
            }
        }

        private void Hit(Player player)
        {
            player.AddCard(GetCard());
        }

        private void Stand(Player player)
        {
            player.CanTurn = false;
        }

        private void EndGame()
        {
            UpDillerHiddenCard();

            if (!GetNotNotifiedPlayers().Any())
            {
                OnGameEnd?.Invoke();
                isGame = false;
                return;
            }

            while (_dealer.CanTurn)
            {
                _dealer.AddCard(GetCard());
            }

            if (_dealer.IsBust())
            {
                Notify(GameResult.Win);
                return;
            }

            if (_dealer.IsBlackJack())
            {
                Notify(GameResult.Lose);
                return;
            }

            foreach (var player in GetNotNotifiedPlayers())
            {
                if (player.IsMore())
                {
                    player.InvokeOnResult(GameResult.Win);
                    continue;
                }

                if (player.IsEquals())
                {
                    player.InvokeOnResult(GameResult.Push);
                    continue;
                }

                player.InvokeOnResult(GameResult.Lose);
            }

            OnGameEnd?.Invoke();
            isGame = false;

            void Notify(GameResult result)
            {
                isGame = false;
                NotifyPlayers(result);
                OnGameEnd?.Invoke();
            }
        }

        private void NotifyPlayers(GameResult result)
        {
            foreach (var player in GetNotNotifiedPlayers())
            {
                player.InvokeOnResult(result);
            }
        }

        private IEnumerable<Player> GetNotNotifiedPlayers()
            => _players.Where(x => !x.isNotifiedResult);

        private void UpDillerHiddenCard()
        {
            _dealer.AddCardHidden(dillerHiddenCard);
            OnDillerUpHiddenCard?.Invoke(dillerHiddenCard);
        }

        private Card GetCard()
        {
            if (!deck.TryPeek(out Card card))
            {
                deck.Reload();
            }
            return card;
        }
    }

    public enum GameResult
    {
        Win = 1,
        Lose,
        Push,
    }

    public enum BlackJackTurn
    {
        Hit,
        Stand,
        Split
    }
}
