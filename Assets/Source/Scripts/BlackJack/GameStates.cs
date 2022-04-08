using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Scripts.BlackJack
{
    [CreateAssetMenu(fileName = "GameStates", menuName = "Xdd/GAME/States", order = 31)]
    public class GameStates : ScriptableObject
    {
        [field: SerializeField] public string TimePostfix { get; private set; }
        [field: SerializeField] public List<GameState> States { get; private set; }
    }

    [Serializable]
    public class GameState
    {
        [field: SerializeField] public string StateName { get; private set; }
        [field: SerializeField] public string DisplayName { get; set; }
    }
}
