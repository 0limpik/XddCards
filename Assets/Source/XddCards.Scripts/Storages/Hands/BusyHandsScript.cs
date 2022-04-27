using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Scripts.Base;
using Xdd.Scripts.BlackJack;
using Xdd.Scripts.UI.BlackJack;

namespace Xdd.Scripts.Hands
{
    internal class BusyHandsScript : MonoBehaviour, ICycleRequired
    {
        public event Action<HandScript> OnInteraction;

        [SerializeField] private List<HandScript> _Hands = new();

        private IHandController controller => cycle.HandController;
        private IUser user;
        private IBJCycle cycle;

        public HandScript[] Hands => AllHands.Where(x => x.IsBusy).ToArray();
        public IEnumerable<HandScript> AllHands => _Hands.Where(x => x.gameObject.activeSelf);

        async void Awake()
        {
            try
            {
                while (true)
                {
                    await Task.Yield();

                    if (this == null) // when exit to editor
                        return;

                    foreach (var hand in AllHands)
                    {
                        if (LastRaycastScript.underMouseLeftClick == hand.gameObject && !hand.IsBusy)
                        {
                            hand.Take();
                            try
                            {
                                await user.Take(hand.GetComponent<BJHandScript>().Hand);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                                hand.Relese();
                            }
                            await TaskEx.Delay(0.1f);
                            break;
                        }
                        if (LastRaycastScript.underMouseRightClick == hand.gameObject && hand.IsBusy)
                        {
                            hand.Relese();
                            try
                            {
                                await user.Release(hand.GetComponent<BJHandScript>().Hand);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                                hand.Take();
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

        void Start()
        {
            Lock(true);
        }

        public async void InitCycle(IBJCycle cycle, IUser user)
        {
            this.cycle = cycle;
            this.user = user;

            controller.OnChangeExecute += OnChangeExecute;

            await TaskEx.Delay(1f);

            var avalibleHands = controller.Hands;
            for (int i = 0; i < avalibleHands.Length; i++)
            {
                var hand = _Hands[i];
                hand.OnInteraction += () => OnInteraction?.Invoke(hand);
            }
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
