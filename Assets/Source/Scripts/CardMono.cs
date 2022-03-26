using System;
using UnityEngine;

[ExecuteAlways]
public class CardMono : MonoBehaviour
{
    public event Action OnCardChange;

    public CardObject card
    {
        get => _card; 
        set
        {
            _card = value;
            OnCardChange?.Invoke();
        }
    }
    [SerializeField] private CardObject _card;

    private Renderer _renderer;

    private void Awake()
    {
        OnCardChange += () => SetMaterial();
        SetMaterial();
    }

    private void SetMaterial()
    {
        _renderer = GetComponent<Renderer>();
        if (card?.material != null)
            _renderer.material = card.material;
    }
}
