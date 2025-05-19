using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Goal");
            MazeGenerator.Instance.ResetGrid();
        }
    }
}
