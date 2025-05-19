using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSwap : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TileSwapBoard Board;
    [SerializeField] private Image _image;

    public Vector2Int Coord { get;  set; }
    public int Id {  get; private set; }
    public void Init(Sprite sprite, Vector2Int coord, int id)
    {
        _image.sprite = sprite;
        Coord = coord;
        Id = id;
    }
    private void Awake() 
    { 
        Board = GetComponentInParent<TileSwapBoard>();
        _image = GetComponentInChildren<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Board.RequestSwap(this);
    }
    public IEnumerator MoveToLocal(Vector3 localPos, float dur = .15f)
    {
        var rt = (RectTransform)transform;
        Vector3 start = rt.localPosition;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            rt.localPosition = Vector3.Lerp(start, localPos, t);
            yield return null;
        }
    }
}
