using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentParent;

    public GameObject infoPanel;
    public TMP_Text infoText;

    [SerializeField] private List<GameObject> uiSlots = new();
    //[SerializeField] private InventorySlotUI slotUI;
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    void Start()
    {
        Inventory.Instance.OnInventoryChanged += OnInventoryChanged;
    }

    void OnDestroy()
    {
        Inventory.Instance.OnInventoryChanged -= OnInventoryChanged;
    }

    void OnInventoryChanged()
    {
        if (gameObject.activeInHierarchy)
        {
            RefreshUI();
        }
    }
    
    private void Reset()
    {
        contentParent = transform.GetComponentInChildren<GridLayoutGroup>().transform;
        GameObject contentPanel = transform.GetChild(0).gameObject;
        infoPanel = contentPanel.GetComponentInChildren<Image>().gameObject;
        infoText = contentPanel.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void RefreshUI()
    {
        foreach (var slot in uiSlots) Destroy(slot);
        uiSlots.Clear();

        foreach (var slotData in Inventory.Instance.Slots)
        {
            var slot = Instantiate(slotPrefab,  contentParent);
            var qty = slot.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var icon = slot.transform.GetChild(1).GetComponent<Image>();
            //slotUI = slot.GetComponent<InventorySlotUI>();
            //slotUI.Setup(slotData.item, slotData.quantity);

            icon.sprite = slotData.item.icon;
            qty.text = slotData.item.stackable ? slotData.quantity.ToString() : "";
            slot.GetComponent<Button>().onClick.AddListener(() => { ShowItemInfo(slotData.item); });

            uiSlots.Add(slot);
        }
    }
    public void ShowItemInfo(ItemSO item)
    {
        infoText.text = $"{item.itemDescription}";
    }
}
