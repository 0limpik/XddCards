using Assets.Source.Model.Enums;

namespace Assets.Source.Model.Games
{
    public class Card : ICard
    {
        public Suits suit { get; set; }
        public Ranks rank { get; set; }

        public override string ToString()
            => $"{suit} {rank}";
    }

    public interface ICard
    {
        Suits suit { get; }
        Ranks rank { get; }
    }
}
