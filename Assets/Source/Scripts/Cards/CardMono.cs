using System;
using System.Threading.Tasks;
using UnityEngine;
using Xdd.Scripts.Base;

namespace Xdd.Scripts.Cards
{
    [ExecuteAlways]
    public class CardMono : MonoBehaviour
    {
        public event Action OnCardChange;

        public float flipTime;

        public CardObject card
        {
            get => _card;
            set
            {
                _card = value;
                SetMaterial();
            }
        }
        [SerializeField] private CardObject _card;

        private Renderer _renderer;

        void Awake()
        {
            SetMaterial();
        }

        public ITaskItem Flip(TaskQueue manager)
        {
            var task = manager.AddTask((x) => FlipCard(), name: $"flip{card:n}");
            OnCardChange?.Invoke();
            return task;
        }


        private void SetMaterial()
        {
            _renderer = GetComponent<Renderer>();
            if (card?.material != null)
                _renderer.material = card.material;
        }

        private async Task FlipCard()
        {
            var flipTime = this.flipTime;

            var startRotation = transform.rotation;
            var startPosition = transform.position;
            var endRotation = startRotation * Quaternion.AngleAxis(180, Vector3.forward);

            if (flipTime != 0)
            {
                while ((flipTime -= Time.deltaTime) > 0)
                {
                    var relation = flipTime / this.flipTime;
                    Rotate(relation);
                    await Task.Yield();
                }
            }
            Rotate(0);
            OnCardChange?.Invoke();
            void Rotate(float relation)
            {
                if (this != null)
                {
                    transform.rotation = Quaternion.Lerp(endRotation, startRotation, relation);
                    transform.position = startPosition + new Vector3(0, CardObject.cardWidth * transform.localScale.z * Mathf.Sin(relation * Mathf.PI), 0);
                }
            }
        }
    }
}