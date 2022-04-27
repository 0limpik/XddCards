using UnityEngine;
using Xdd.Network.Clients.Cycles.BlackJack;
using Xdd.Network.Lobby;
using Xdd.Scripts.BlackJack;
using Xdd.Scripts.UI;

namespace Xdd.Network.Scripts.Cycles.BlackJack
{
    [RequireComponent(typeof(GameManager))]
    internal class BJCycleNetwork : MonoBehaviour
    {
        private GameManager gameManager;

        [SerializeField] private GameUIScript UIScript;

        BJCycleClient client;

        async void Awake()
        {
            client = await BJCycleClient.New(LobbyScript.user);
            gameManager = this.GetComponent<GameManager>();

            gameManager.cycle = client;

            gameManager.InitCycle(client, LobbyScript.user);

            client.OnMessage += (message) => UIScript.DisplayMessage(message, Color.red);
        }
    }
}
