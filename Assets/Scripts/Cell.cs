using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject topWall, bottomWall, leftWall, rightWall, frontWall, backWall;
    public bool visited = false;
    public int x, y, z;
    public int id;

    public void RemoveWall(Vector3 direction)
    {
        if (direction == Vector3.forward) if (frontWall != null) frontWall.SetActive(false);
        if (direction == Vector3.back) if (backWall != null) backWall.SetActive(false);
        if (direction == Vector3.left) if (leftWall != null) leftWall.SetActive(false);
        if (direction == Vector3.right) if (rightWall != null) rightWall.SetActive(false);
        if (direction == Vector3.up) if (topWall != null) topWall.SetActive(false);
        if (direction == Vector3.down) if (bottomWall != null) bottomWall.SetActive(false);
    }
}
