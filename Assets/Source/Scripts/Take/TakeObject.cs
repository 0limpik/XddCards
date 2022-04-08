using System;
using UnityEngine;

namespace Xdd.Scripts.Take
{
    [CreateAssetMenu(fileName = "Take", menuName = "Xdd/TAKE/Object", order = 21)]
    internal class TakeObject : ScriptableObject
    {
        [SerializeField] private TakeStateParam available;
        [SerializeField] private TakeStateParam busy;
        [SerializeField] private TakeStateParam press;
        [SerializeField] private TakeStateParam release;

        public TakeStateParam[] AllStates => new TakeStateParam[] { available, busy, press, release };
    }

    [Serializable]
    internal class TakeStateParam
    {
        public Material material;
        public Color lightColor;
        public float lightRange;
        public TakeState state;
    }

    internal enum TakeState
    {
        Available,
        Busy,
        Press,
        Release
    }

}
