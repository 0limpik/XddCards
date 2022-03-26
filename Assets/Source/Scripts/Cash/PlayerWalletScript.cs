using Assets.Source.Scripts.Cash;
using UnityEngine;

public class PlayerWalletScript : MonoBehaviour
{
    PlayerWallet wallet;

    void Start()
    {
        wallet = new PlayerWallet(10000m);
    }
}
