using System.Threading.Tasks;
using UnityEngine;

[ExecuteAlways]
public class CardMono : MonoBehaviour
{
    public CardObject card { get => _card; set { _card = value; Start(); } }
    [SerializeField] private CardObject _card;

    private Renderer _renderer;

    [SerializeField] private float flipTime;

    private float cardWidnth => this.transform.localScale.y * 1.66f;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (card?.material != null)
            _renderer.material = card.material;
    }

    public async Task Flip()
    {
        var flipTime = this.flipTime;
        var startRotation = this.transform.rotation;
        var startPosition = this.transform.position;
        var flippedRot = startRotation * Quaternion.AngleAxis(180, Vector3.forward);

        while ((flipTime -= Time.deltaTime) > 0)
        {
            var relation = flipTime / this.flipTime;

            this.transform.rotation = Quaternion.Lerp(flippedRot, startRotation, relation);
            this.transform.position = startPosition + new Vector3(0, Mathf.Sin(relation * Mathf.PI), 0);
            await Task.Yield();
        }
    }
}
