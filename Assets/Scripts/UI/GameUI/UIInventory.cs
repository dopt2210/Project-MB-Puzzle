using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button[] buttons;

    private InventorySystem inventory => InventorySystem.Instance;
    private void OnEnable()
    {
        InventorySystem.Instance.OnItemAdd += UpdateSlot;

    }
    private void OnDisable()
    {
        InventorySystem.Instance.OnItemAdd -= UpdateSlot;
    }
    
    private void Reset()
    {
        contentParent = transform.GetComponentInChildren<GridLayoutGroup>().transform;
        GameObject contentPanel = transform.GetChild(1).gameObject;
        infoText = contentPanel.GetComponentInChildren<TextMeshProUGUI>();
        buttons = contentPanel.GetComponentsInChildren<Button>();
    }
    private void Update()
    {
        if (InputManager.Instance.Action.OpenItem) OpenBag();
        else if (InputManager.Instance.Action.CloseItem) CloseBag();
    }
    public void RefreshUI()
    {
        if (!gameObject.activeSelf) return;

        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform slot = contentParent.GetChild(i);
            InventoryData data = inventory.GetItemAt(i);
            SetupSlotUI(slot, data);
        }
    }

    private void UpdateSlot(int index)
    {
        if (index < 0 || index >= contentParent.childCount) return;
        Transform slot = contentParent.GetChild(index);
        InventoryData data = inventory.GetItemAt(index);
        SetupSlotUI(slot, data);
    }

    private void SetupSlotUI(Transform slot, InventoryData data)
    {
        // Xóa item cũ nếu khác loại hoặc số lượng 0
        foreach (Transform child in slot)
            Destroy(child.gameObject);

        if (data == null) return;

        GameObject itemGO = Instantiate(itemPrefab, slot);
        InventoryItem item = itemGO.GetComponent<InventoryItem>();
        item.tParrent = slot;
        item.itemData = data;
        item.image.sprite = data.item.icon;
        item.qty.text = data.quantity.ToString();

        Button btn = item.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => ShowItemInfo(data.item));
    }

    public void ShowItemInfo(ItemSO item)
    {
        infoText.text = $"{item.itemDescription}";
    }
    public void OpenBag()
    {
        CameraSwitch.Instance.ShowInventoryCam();
        MouseLock.Instance.UnlockMouse();

    }
    public void CloseBag()
    {
        CameraSwitch.Instance.ShowGameplayCam();
        MouseLock.Instance.LockMouse();
    }
    public void Toggle()
    {
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);
        string inputMap = isActive ? "Player" : "UI";
        InputManager.InputPlayer.SwitchCurrentActionMap(inputMap);
        MouseLock.Instance.AutoHandleMouseLockByPause(!isActive);

    }
}