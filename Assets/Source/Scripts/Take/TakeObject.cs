using System;
using UnityEngine;

namespace Assets.Source.Scripts.Peek
{
    [CreateAssetMenu(fileName = "Take", menuName = "Xdd/TAKE/Object", order = 21)]
    public class TakeObject : ScriptableObject
    {
        [SerializeField] private TakeStateParam available;
        [SerializeField] private TakeStateParam busy;
        [SerializeField] private TakeStateParam press;
        [SerializeField] private TakeStateParam release;

        public TakeStateParam[] AllStates => new TakeStateParam[] { available, busy, press, release };
    }

    [Serializable]
    public class TakeStateParam
    {
        public Material material;
        public Color lightColor;
        public float lightRange;
        public TakeState state;
    }

    public enum TakeState
    {
        Available,
        Busy,
        Press,
        Release
    }

}
