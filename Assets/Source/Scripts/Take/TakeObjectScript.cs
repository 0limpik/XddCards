using System.Linq;
using UnityEngine;
using Xdd.Scripts.Base;

namespace Xdd.Scripts.Take
{
    [RequireComponent(typeof(MeshRenderer))]
    internal class TakeObjectScript : MonoBehaviour
    {
        [SerializeField] private TakeObject takeObject;

        public bool Disable
        {
            set
            {
                _meshRenderer.enabled = !value;
                _collider.enabled = !value;
            }
        }

        public TakeState State
        {
            get => _State;
            set
            {
                if (value != _State)
                {
                    _State = value;
                    SetState(value);
                }
            }
        }
        [SerializeField] private TakeState _State;

        private Light _light;
        private Renderer _renderer;
        private MeshRenderer _meshRenderer;
        private Collider _collider;

        private float intensity;

        void Awake()
        {
            _light = this.GetComponentInChildren<Light>();
            _renderer = this.GetComponent<Renderer>();
            _meshRenderer = this.GetComponent<MeshRenderer>();
            _collider = this.GetComponent<Collider>();
            SetState(_State);

            intensity = _light.intensity;
        }

        private void SetState(TakeState state)
        {
            var stateParam = takeObject.AllStates.First(x => x.state == state);

            _renderer.material = stateParam.material;
            _light.color = stateParam.lightColor;
            _light.range = stateParam.lightRange;
        }

        private bool lastState;


        void Update()
        {
            _light.intensity = intensity + Mathf.PingPong(Time.time * 3, 5);
        }

        void LateUpdate()
        {
            var state = false;

            if (LastRaycastScript.underMouse == this.gameObject)
            {
                state = true;
            }

            if (state != lastState)
            {
                _light.enabled = state;
                lastState = state;
            }
        }
    }
}
