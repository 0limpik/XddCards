using System.Threading.Tasks;
using Assets.Source.Network.Clients;
using Assets.Source.Network.Clients.Games.BlackJack;
using Assets.Source.Network.Clients.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XddCards.Grpc;

namespace Assets.Source.Network.Lobby
{
    internal class LobbyScript : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        async Task Awake()
        {
            var client = Client.GetClient<Greeter.GreeterClient>();

            var x = await client.SayHelloAsync(new HelloRequest { Name = "Unity" });

            Debug.Log(x.Message);

            var nickname = $"Player {Random.Range(1000, 9999)}";

            inputField.text = nickname;

            AuthClient.Init(nickname);
        }

        public async void CreateGame()
        {
            var client = new GamesClient();
            await client.CreateGame();

            SceneManager.LoadScene("BlackJack");
        }
    }
}
