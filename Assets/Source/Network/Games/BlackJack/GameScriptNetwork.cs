using System;
using System.Threading.Tasks;
using Assets.Source.Network.Clients.Games.BlackJack;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Model.Games;
using Xdd.Scripts.BlackJack;

namespace Assets.Source.Network.Games.BlackJack
{
    [RequireComponent(typeof(GameScript))]
    internal class GameScriptNetwork : MonoBehaviour, IGameController
    {
        private GameScript gameManager;

        public IHand dealerHand => gameClient.dealerHand;

        public IHand playerHand => gameClient.playersHand[0];

        public event Action<bool> OnChangeExecute;
        public event Action<ICard> OnDillerUpHiddenCard
        {
            add => gameClient.OnDillerUpHiddenCard += value;
            remove => gameClient.OnDillerUpHiddenCard -= value;
        }
        public event Action OnGameEnd
        {
            add => gameClient.OnGameEnd += value;
            remove => gameClient.OnGameEnd -= value;
        }

        private GameClient gameClient;

        [SerializeField] private BJHandScript handScript;
        [SerializeField] private BJHandScript dealerHandScript;

        void Awake()
        {
            gameManager = this.GetComponent<GameScript>();
        }

        async void Start()
        {
            gameClient = new GameClient();

            await gameClient.Init(1);

            gameManager.controller = this;

            gameManager.InitCycle(null, null);

            gameManager.StartGame();
        }

        async Task IGameController.StartAsync()
        {
            await gameClient.Start();
        }

        void IGameController.Start()
        {
            gameClient.Start().RunSynchronously();
        }
    }
}
