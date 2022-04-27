using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xdd.Network.Clients;
using Xdd.Network.Clients.Cycles.BlackJack;
using Xdd.Network.Clients.Games;
using Xdd.Network.Clients.Lobby;
using XddCards.Grpc;

namespace Xdd.Network.Lobby
{
    internal class LobbyScript : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] RectTransform panel;
        [SerializeField] Button Button;

        public static UserClient user;

        BlackJackClient client;

        async void Awake()
        {
            var client = GrpcClient<Greeter.GreeterClient>.GetNewClient();

            var x = await client.SayHelloAsync(new HelloRequest { Name = "Unity" });

            Debug.Log(x.Message);

            var nickname = $"Player {Random.Range(1000, 9999)}";

            inputField.text = nickname;

            await AuthClient.Init(nickname);
            this.client = new BlackJackClient();

            var games = await this.client.GetAll();

            foreach (var game in games)
            {
                var button = GameObject.Instantiate<Button>(Button, panel.transform);
                button.GetComponentInChildren<TMP_Text>().text = game.ToString();
                button.onClick.AddListener(() => ConnectToGame(game));
            }
        }

        public async void CreateGame()
        {
            var response = await client.Create();
            user = response;
            SceneManager.LoadScene("BlackJack");
        }

        public async void ConnectToGame(int id)
        {
            var response = await client.Connect(id);
            user = response;
            SceneManager.LoadScene("BlackJack");
        }
    }
}
