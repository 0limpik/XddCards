using System;
using System.Threading.Tasks;
using Assets.Source.Scripts.Peek;
using UnityEngine;

namespace Assets.Source.Scripts.BlackJack.Hand
{
    [RequireComponent(typeof(TakeObjectScript))]
    public class HandScript : MonoBehaviour
    {
        public event Action OnInteraction;

        public bool IsBusy { get; private set; }

        public bool Lock
        {
            get => _Lock;
            set
            {
                if(value != _Lock)
                {
                    _Lock = value;

                    takeScript.Disable = _Lock; 
                }
            }
        }
        private bool _Lock;

        private TakeObjectScript takeScript;

        async void Awake()
        {
            takeScript = this.GetComponent<TakeObjectScript>();
            takeScript.State = TakeState.Available;

            while (true)
            {
                if (this == null)
                    return;

                if (LastRaycastScript.underMouseLeftClick == this.gameObject)
                {
                    Take();
                    await TaskEx.Delay(0.1f);
                }
                if (LastRaycastScript.underMouseRightClick == this.gameObject)
                {
                    Relese();
                    await TaskEx.Delay(0.1f);
                }
                await Task.Yield();
            }
        }

        public void Take()
        {
            if (Lock)
                throw new Exception("IsLock");

            if (IsBusy)
                throw new Exception("Already busy");

            IsBusy = true;
            OnInteraction?.Invoke();
        }

        public void Relese()
        {
            if (Lock)
                throw new Exception("IsLock");

            if (!IsBusy)
                throw new Exception("Already relese");

            IsBusy = false;
            OnInteraction?.Invoke();
        }


        private TakeState lastState;
        private void LateUpdate()
        {
            var state = lastState;
            if (LastRaycastScript.underMouse == this.gameObject)
            {
                if (IsBusy)
                {
                    state = TakeState.Release;
                }
                else
                {
                    state = TakeState.Press;
                }

            }
            else
            {
                if (IsBusy)
                {
                    state = TakeState.Busy;
                }
                else
                {
                    state = TakeState.Available;
                }
            }

            if (state != lastState)
            {
                takeScript.State = state;
                lastState = state;
            }
        }
    }
}
