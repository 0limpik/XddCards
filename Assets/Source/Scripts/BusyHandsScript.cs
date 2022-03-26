using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Source.Scripts.BlackJack.Hand;
using UnityEngine;

public class BusyHandsScript : MonoBehaviour
{
    public event Action OnInteraction;

    [SerializeField] private List<HandScript> _Hands = new();

    public List<HandScript> Hands => _Hands.Where(x => x.gameObject.activeSelf).Where(x => x.IsBusy).ToList();
    public List<HandScript> AllHands => _Hands.Where(x => x.gameObject.activeSelf).ToList();

    void Awake()
    {
        foreach (var hand in _Hands)
        {
            hand.OnInteraction += () => OnInteraction?.Invoke();
        }
    }

}
