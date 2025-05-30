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
        if (InputManager.Instance.Action.OpenItem) { OpenBag(); }
        else if (InputManager.Instance.Action.CloseItem) { CloseBag(); }
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
        foreach (Transform child in slot)
            Destroy(child.gameObject);

        if (data == null) return;

        GameObject itemGO = Instantiate(itemPrefab, slot);
        InventoryItem item = itemGO.GetComponent<InventoryItem>();
        item.tParrent = slot;
        item.itemData = data;
        item.image.sprite = data.item.icon;
        item.qty.text = data.item.itemName;

        Button btn = item.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            ShowItemInfo(data.item);
            ShowButtonInfo(data.item);
            });
    }

    public void ShowItemInfo(ItemSO item)
    {
        infoText.text = $"{item.itemDescription}";
    }
    public void ShowButtonInfo(ItemSO item)
    {
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(() => PlayPuzzleFromItem(item));

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => UIHandler.Instance.ShowHint(item.itemDescription));

    }
    public void OpenBag()
    {
        CameraSwitch.Instance.SwitchInventoryCamera();

    }
    public void CloseBag()
    {
        CameraSwitch.Instance.SwitchPlayerCamera();
    }
    private void PlayPuzzleFromItem(ItemSO item)
    {
        var puzzleData = item.puzzleData;

        if (puzzleData == null)
        {
            Debug.LogWarning($"Item {item.name} has no level data assigned.");
            return;

        }

        switch (item.puzzleType)
        {
            case PuzzleType.TilePuzzle:
                PuzzleManager.Instance.PlayTileSwapPuzzle(puzzleData as TileSwapSO);
                break;
            case PuzzleType.PairPuzzle:
                PuzzleManager.Instance.PlayPairPathPuzzle(puzzleData as PairPathSO);
                break;
            case PuzzleType.WordlePuzzle:
                PuzzleManager.Instance.PlayWordlePuzzle(puzzleData as WordleSO);
                break;
            default:
                Debug.LogWarning($"No puzzle assigned to item: {item.itemName}");
                break;
        }
    }
}
    //public void RefreshUI()
    //{
    //    if (!gameObject.activeSelf) return;

    //    for (int i = 0; i < contentParent.childCount; i++)
    //    {
    //        Transform slot = contentParent.GetChild(i);
    //        InventoryData data = inventory.GetItemAt(i);
    //        SetupSlotUI(slot, data);
    //    }
    //}