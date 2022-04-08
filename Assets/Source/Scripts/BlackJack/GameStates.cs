using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xdd.Scripts.BlackJack
{
    [CreateAssetMenu(fileName = "GameStates", menuName = "Xdd/GAME/States", order = 31)]
    internal class GameStates : ScriptableObject
    {
        [field: SerializeField] public string TimePostfix { get; private set; }
        [field: SerializeField] public List<GameState> States { get; private set; }
    }

    [Serializable]
    internal class GameState
    {
        [field: SerializeField] public string StateName { get; private set; }
        [field: SerializeField] public string DisplayName { get; set; }
    }
}
