using System;
using UnityEngine;

namespace Assets.Source.Scripts.Bets
{
    [CreateAssetMenu(fileName = "PossibleBets", menuName = "Xdd/BETS/PossibleBets", order = 13)]
    public class PossibleBets : ScriptableObject
    {
        [field: SerializeField]
        public string betsPostfix { get; private set; }

        [field: SerializeField]
        public PossibleBet[] possibleBets { get; private set; }
    }

    [Serializable]
    public class PossibleBet
    {
        public decimal Amount => (decimal)_Amount;
        [SerializeField]
        private float _Amount;

        [field: SerializeField]
        public Sprite Sprite { get; private set; }
    }
}
