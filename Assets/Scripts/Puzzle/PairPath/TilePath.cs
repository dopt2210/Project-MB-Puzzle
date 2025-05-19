using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TilePath : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private Image _image;

    public Vector2Int Coord {  get; private set; }
    public int Id { get; private set; }
    public bool IsBlank { get; private set; }
    public Color TileColor { get; private set; }
    private void Awake()
    {
        _image = GetComponentInChildren<Image>();
    }
    // Blank
    public void InitBlank(Vector2Int coord)
    {
        Coord = coord;
        IsBlank = true;
        Id = -1;
    }

    // Tile
    public void InitTile(Vector2Int coord, int pairId, Color color)
    {
        Coord = coord;
        Id = pairId;
        IsBlank = false;

        color.a = 255f;
        _image.color = color;
        TileColor = color;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        LineDraw.Instance.CellClicked(this);
        Debug.Log("OnPointerClick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LineDraw.Instance.CellHovered(this);
    }
}