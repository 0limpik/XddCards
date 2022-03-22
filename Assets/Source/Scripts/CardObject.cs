using Assets.Source.Model.Enums;
using Assets.Source.Model.Games;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CARD/OBJECT", order = 100000)]
public class CardObject : ScriptableObject, ICard
{
    public Material material;

    public Suits suit => _suit;
    [SerializeField] public Suits _suit;

    public Ranks rank => _rank;
    [SerializeField] public Ranks _rank;


    public static float cardWidth = 1.66f;
}

