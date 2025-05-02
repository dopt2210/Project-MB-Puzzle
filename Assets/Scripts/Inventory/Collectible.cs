using UnityEngine;

public class Collectible : MonoBehaviour
{
    public ItemSO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool success = Inventory.Instance.AddItem(itemData);
            if (success)
            {
                Destroy(gameObject);
            }
        }
    }
}
