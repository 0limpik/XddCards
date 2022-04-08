using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.Base;
using Xdd.Scripts.UI.BlackJack;

namespace Xdd.Scripts.Hands
{
    internal class BusyHandsScript : MonoBehaviour, ICycleRequired
    {
        public event Action<HandScript> OnInteraction;

        [SerializeField] private List<HandScript> _Hands = new();

        private HandController controller => cycle.handController;
        private User user;
        private BJCycle cycle;

        public HandScript[] Hands => AllHands.Where(x => x.IsBusy).ToArray();
        public IEnumerable<HandScript> AllHands => _Hands.Where(x => x.gameObject.activeSelf);

        async void Awake()
        {
            try
            {
                foreach (var hand in _Hands)
                {
                    hand.OnInteraction += () => OnInteraction?.Invoke(hand);
                }

                while (true)
                {
                    await Task.Yield();

                    if (this == null) // when exit to editor
                        return;

                    foreach (var hand in AllHands)
                    {
                        if (LastRaycastScript.underMouseLeftClick == hand.gameObject)
                        {
                            if (!hand.IsBusy)
                            {
                                hand.Take();
                                try
                                {
                                    user.Take();
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    hand.Relese();
                                }
                            }
                            await TaskEx.Delay(0.1f);
                            break;
                        }
                        if (LastRaycastScript.underMouseRightClick == hand.gameObject)
                        {
                            if (hand.IsBusy)
                            {
                                hand.Relese();
                                try
                                {
                                    user.Release();
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                    hand.Take();
                                }
                            }
                            await TaskEx.Delay(0.1f);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void InitCycle(BJCycle cycle, User user)
        {
            this.cycle = cycle;
            this.user = user;

            controller.OnChangeExecute += OnChangeExecute;
        }

        public void Lock(bool _lock)
        {
            foreach (var hand in AllHands)
            {
                hand.Lock = _lock;
            }
        }

        private void OnChangeExecute(bool execute)
        {
            Lock(!execute);
        }
    }
}
