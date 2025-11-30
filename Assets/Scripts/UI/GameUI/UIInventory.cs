using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button[] buttons;

    private ItemSO _currentShowingItem;

    private InventorySystem inventory => InventorySystem.Instance;
    private void OnEnable()
    {
        InventorySystem.OnItemAdd += UpdateSlot;
        BaseBoard.OnSolved += HandlePuzzleSolved;

    }
    private void OnDisable()
    {
        InventorySystem.OnItemAdd -= UpdateSlot;
        BaseBoard.OnSolved -= HandlePuzzleSolved;
    }

    private void Reset()
    {
        contentParent = transform.GetComponentInChildren<GridLayoutGroup>().transform;
        GameObject contentPanel = transform.GetChild(2).gameObject;
        infoText = contentPanel.GetComponentInChildren<TextMeshProUGUI>();
        buttons = contentPanel.GetComponentsInChildren<Button>(true);
    }
    private void Update()
    {
        if (InputManager.Instance.Action.OpenItem) { OpenBag(); }
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
    private void HandlePuzzleSolved(string uid, bool solved)
    {
        if (!solved || _currentShowingItem == null) return;

        string currentUID = GetUIDFromItemSO(_currentShowingItem);
        if (uid == currentUID)
        {
            buttons[2].gameObject.SetActive(true);
        }
    }

    public void ShowItemInfo(ItemSO item)
    {
        _currentShowingItem = item;

        infoText.text = $"{item.itemDescription}";
        string uid = GetUIDFromItemSO(item);
        bool isSolved = CheckIfPuzzleSolved(uid);
        Debug.Log($"Item: {item.itemName}, UID: {uid}, Solved: {isSolved}");
        buttons[2].gameObject.SetActive(isSolved);
    }
    public void ShowButtonInfo(ItemSO item)
    {
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(() => PlayPuzzleFromItem(item));

        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => UIHandler.Instance.ShowHint(item.itemDescription));

        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(() => {
            var targetCell = Finding(item);
            if (targetCell == null)
            {
                NotifyManager.Instance.Notify("You not in the maze to use this item");
                return;
            }
            else GameManager.Instance.FindPlayerToTarget(Finding(item));
        });

    }
    private Cell Finding(ItemSO item)
    {
        if ((int)item.algorithmType != GameManager.Instance.CurrentLevel - 1)
            return null;
        var target = MazeGenerator.KeyPoint.GetKeyPoints(item.algorithmType).FirstOrDefault();
        if(target == null)
        {
            target = GameManager.Instance.GetTargetCell();
        }
        return target;
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
    private string GetUIDFromItemSO(ItemSO item)
    {
        switch (item.puzzleType)
        {
            case PuzzleType.TilePuzzle:
                return (item.puzzleData as TileSwapSO)?.uniqueId ?? "unknown";
            case PuzzleType.PairPuzzle:
                return (item.puzzleData as PairPathSO)?.uniqueId ?? "unknown";
            case PuzzleType.WordlePuzzle:
                return (item.puzzleData as WordleSO)?.uniqueId ?? "unknown";
            default:
                return string.Empty;
        }
    }
    private bool CheckIfPuzzleSolved(string uid)
    {
        var data = GameDataManager.Instance.GetGameData();
        if (data == null)
            return false;

        return data.puzzleStates.TryGetValue(uid, out bool solved) && solved;
    }
}