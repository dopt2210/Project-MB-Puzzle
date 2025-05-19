using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject topWall, bottomWall, leftWall, rightWall, frontWall, backWall;
    public bool visited = false;
    public bool flagVisited = false;
    public int x, y, z;
    public int setID;

    public void RemoveWall(Vector3 direction)
    {
        if (direction == Vector3.forward) if (frontWall != null) frontWall.SetActive(false);
        if (direction == Vector3.back) if (backWall != null) backWall.SetActive(false);
        if (direction == Vector3.left) if (leftWall != null) leftWall.SetActive(false);
        if (direction == Vector3.right) if (rightWall != null) rightWall.SetActive(false);
        if (direction == Vector3.up) if (topWall != null) topWall.SetActive(false);
        if (direction == Vector3.down) if (bottomWall != null) bottomWall.SetActive(false);
    }
    public void HighlightForMiniMap(Color color)
    {
        SpriteRenderer rendererColor = bottomWall.GetComponentInChildren<SpriteRenderer>();
        
        if (rendererColor != null)
        {
            rendererColor.color = color;
        }
    }
    public void ResetState()
    {
        topWall?.SetActive(true);
        bottomWall?.SetActive(true);
        leftWall?.SetActive(true);
        rightWall?.SetActive(true);
        frontWall?.SetActive(true);
        backWall?.SetActive(true);

        visited = false;
        flagVisited = false;
        setID = 0;

        // Reset lại màu minimap nếu có
        SpriteRenderer cellRenderer = bottomWall?.GetComponentInChildren<SpriteRenderer>();
        if (cellRenderer != null)
        {
            cellRenderer.color = Color.white;
        }
    }
    public Vector3 GetWorldPosition(float scale)
    {
        float x = this.x * scale;
        float z = this.z * scale;
        Vector3 pos = new Vector3(x + scale / 2, this.y + 1, z + scale / 2);
        return pos;
    }

}
