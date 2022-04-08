using UnityEngine;
using Xdd.Model.Enums;
using Xdd.Model.Games;

namespace Xdd.Scripts.Cards
{
    [CreateAssetMenu(fileName = "Card", menuName = "Xdd/CARD/Object", order = 12)]
    public class CardObject : ScriptableObject, ICard
    {
        public Material material;

        public Suits suit => _suit;
        [SerializeField] public Suits _suit;

        public Ranks rank => _rank;
        [SerializeField] public Ranks _rank;

        public static float cardWidth = 3.25f;

        public string ToString(string format)
        {
            if (format == "n")
                return $"{suit} {rank}";

            return ToString();
        }
    }
}