using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// AI movement with fog-of-war exploration:
/// - AI chỉ biết các cell kề cạnh hiện tại (chưa biết toàn bộ bản đồ)
/// - Mỗi cell được đến sẽ khám phá các cell xung quanh
/// - Vừa đi vừa vẽ bản đồ vừa quyết định (greedy exploration)
/// </summary>
public class CharacterAIMovement : MonoBehaviour
{
    public static event System.Action OnAIGoalReached;
    public MazeSO mazeSO;
    public Transform target;
    public float arriveRadius = 0.5f; // Bán kính coi như đã tới cell
    public static bool AiEnabled = false; // Cho phép bật/tắt AI runtime
    // Fog-of-war: known cells that AI has discovered
    private HashSet<Cell> knownCells = new HashSet<Cell>();
    private HashSet<Cell> visitedCells = new HashSet<Cell>();

    // Current target cell based on limited knowledge
    private Cell currentTargetCell;
    private Cell previousCell; // Track previous cell để tránh quay lại
    private Cell targetCell; // Cell chứa target (nếu biết)
    private Vector3 nextWaypoint;
    private float finishTime = 0f;
    private float totalTime = 0f;
    private bool isFinished = false;
    private bool hasReachedCurrentTarget = false; // Flag để chỉ quyết định khi đã đến
    private bool hasRevealedGoal = false; // AI đã được reveal đích chưa

    private CharacterController controller;
    [SerializeField] private Animator animator;
    private Vector3 velocity;
    [SerializeField] private float scale;
    [SerializeField] private PlayerSO playerSO;
    // Không dùng hash animator khác ngoài Speed

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        playerSO = Resources.Load<PlayerSO>("Scriptable/PlayerSO");

    }

    void Start()
    {
        mazeSO = GameManager.Instance._mazeSO;
        target = GameManager.Instance.GoalObj.transform;
        controller = GetComponent<CharacterController>();
        scale = GameManager.Instance._mazeSO.GetSizeScale;
        AiEnabled = true;
        hasRevealedGoal = false;
        // Discover initial cell and its neighbors
        var startCell = MazeTools.GetCellFromGameObject(gameObject, MazeGenerator.MazeGrid, mazeSO.BoxSize, scale);
        if (startCell != null)
        {
            DiscoverCell(startCell);
            visitedCells.Add(startCell); // QUAN TRỌNG: Mark startCell đã thăm
            currentTargetCell = startCell;
            previousCell = null; // Start với null, không phải startCell
            nextWaypoint = startCell.GetWorldPosition(scale);
            hasReachedCurrentTarget = true; // Bắt đầu đã ở cell
        }
        targetCell = MazeTools.GetCellFromGameObject(target.gameObject, MazeGenerator.MazeGrid, mazeSO.BoxSize, scale);
        // Debug.Log("Initial targetCell: " + (targetCell != null ? $"({targetCell.x},{targetCell.y},{targetCell.z})" : "null"));

    }

    void Update()
    {
        if (AiEnabled == false)
        {
            StopAI();
            return;
        }

        if (target == null || currentTargetCell == null){
            return;
        }

        // AI Difficulty: Reveal goal after time threshold
        if (!hasRevealedGoal && targetCell != null && !knownCells.Contains(targetCell))
        {

            float threshold = GameManager.Instance.CurrentLevel <= 5 ? 30f : 60f;
            if (UIInformation.timePlay >= threshold)
            {
                hasRevealedGoal = true;
                addKnowncells();
                
                NotifyManager.Instance.Notify("AI has found goal! you need get faster");
                StopAI(3f);
            }
        }

        if(targetCell != null && visitedCells.Contains(targetCell))
        {
            // Debug.Log("targetCell đã được thăm" + (targetCell != null ? $"({targetCell.x},{targetCell.y},{targetCell.z})" : "null"));
            if (!isFinished)
            {
                isFinished = true;
                OnAIGoalReached?.Invoke();
                // Debug.Log("AI đã hoàn thành mê cung!");
            }
            StopAI(1f);
            return;
        }

        // Kiểm tra đã đến cell target chưa
        Vector3 toWaypoint = nextWaypoint - transform.position;
        Vector3 planar = new Vector3(toWaypoint.x, 0f, toWaypoint.z);
        
        if (planar.sqrMagnitude <= arriveRadius * arriveRadius && !hasReachedCurrentTarget)
        {
            hasReachedCurrentTarget = true;
            var reachedCell = MazeTools.GetCellFromGameObject(gameObject, MazeGenerator.MazeGrid, mazeSO.BoxSize, scale);
            if (reachedCell != null)
            {
                visitedCells.Add(reachedCell);
                DiscoverCell(reachedCell);

                // Debug.Log($"Đã đến cell ({reachedCell.x}, {reachedCell.y}, {reachedCell.z})");
            }
        }
        
        // CHỈ quyết định khi đã thực sự đến cell mục tiêu
        if (hasReachedCurrentTarget)
        {
            hasReachedCurrentTarget = false;
            DecideNextMove();
        }

        MoveTowardWaypoint();
    }
    void addKnowncells()
    {
        foreach(var p in MazeGenerator.MazeGrid)
        {
            knownCells.Add(p);
        }
    }
    /// <summary>
    /// Khám phá cell và tất cả các cell kề cạnh của nó
    /// Gọi mỗi khi AI ĐẾN cell để discover neighbors
    /// </summary>
    void DiscoverCell(Cell cell)
    {
        if (cell == null) return;

        // Add cell hiện tại vào known nếu chưa có
        if (!knownCells.Contains(cell))
        {
            knownCells.Add(cell);
        }

        // LUÔN discover neighbors mỗi khi đến cell (không check đã known)
        var neighbors = MazeTools.GetNeighborsByCondition(cell, MazeGenerator.MazeGrid, mazeSO.BoxSize, null);
        foreach (var neighbor in neighbors)
        {
            if (!knownCells.Contains(neighbor) && !MazeTools.HasWallBetween(cell, neighbor))
            {
                knownCells.Add(neighbor);
            }
        }
    }

    /// <summary>
    /// Lựa chọn cell tiếp theo dựa trên những gì AI biết
    /// Ưu tiên: 1) Hướng về phía target nếu biết, 2) Cell chưa khám phá, 3) Random từ known cells
    /// </summary>
    void DecideNextMove()
    {
        var currentCell = MazeTools.GetCellFromGameObject(gameObject, MazeGenerator.MazeGrid, mazeSO.BoxSize, scale);
        if (currentCell == null) return;

        // Lấy tất cả neighbor có thể đi được từ cell hiện tại (trong known cells)
        var walkableNeighbors = new List<Cell>();
        foreach (var neighbor in MazeTools.GetNeighborsByCondition(currentCell, MazeGenerator.MazeGrid, mazeSO.BoxSize, null))
        {
            if (knownCells.Contains(neighbor) && !MazeTools.HasWallBetween(currentCell, neighbor))
            {
                walkableNeighbors.Add(neighbor);
            }
        }

        if (walkableNeighbors.Count == 0)
        {
            currentTargetCell = currentCell;
            return;
        }

        // Chiến lược: Ưu tiên di chuyển về phía target (nếu biết)
        // Truyền walkableNeighbors, để pickGreedyCell tự quyết định filter previousCell
        Cell nextCell = pickGreedyCell(currentCell, walkableNeighbors);
        
        previousCell = currentCell; // Lưu lại cell hiện tại
        currentTargetCell = nextCell;
        nextWaypoint = nextCell.GetWorldPosition(scale);
        
    }

    /// <summary>
    /// Chọn cell tiếp theo theo thứ tự ưu tiên (đơn giản hơn):
    /// 1. UU TIÊN 1: Nếu biết đích → dùng PathFinding_Astar để tìm đường tối ưu
    /// 2. UU TIÊN 2: Nếu không có đường → ưu tiên cell chưa đi thăm từ danh sách known cells
    /// 3. UU TIÊN 3: Còn lại → chọn ngẫu nhiên
    /// </summary>
    Cell pickGreedyCell(Cell current, List<Cell> options)
    {
        if (options.Count == 0)
        {
            return current;
        }

        // ƯU TIÊN 1: Nếu biết vị trí đích, dùng PathFinding_Astar để tìm đường
        if (targetCell != null && knownCells.Contains(targetCell))
        {
            // Debug.Log($"uu tiên 1: AI biết đích ({targetCell.x},{targetCell.y},{targetCell.z}), dùng A* từ ({current.x},{current.y},{current.z})");
            var pathToTarget = new PathFinding_Astar(mazeSO).FindPath(current, targetCell);
            
            if (pathToTarget != null && pathToTarget.Count > 0)
            {
                // Debug.Log($"A* tìm thấy đường {pathToTarget.Count} bước");
                // Path đầu tiên là cell tiếp theo (vì ReconstructPath không bao gồm start)
                Cell nextStep = pathToTarget[0];
                // Debug.Log($"NextStep từ A*: ({nextStep.x},{nextStep.y},{nextStep.z})");
                
                if (options.Contains(nextStep))
                {
                    // Debug.Log($"NextStep có trong options → đi theo A*");
                    return nextStep;
                }
                else
                {
                    // Debug.Log($"NextStep KHÔNG trong options ({options.Count} options). Chọn gần target nhất.");
                    // Chọn neighbor gần targetCell nhất
                    Cell bestOption = options[0];
                    float bestDist = Vector3.Distance(bestOption.GetWorldPosition(scale), targetCell.GetWorldPosition(scale));
                    foreach (var opt in options)
                    {
                        float dist = Vector3.Distance(opt.GetWorldPosition(scale), targetCell.GetWorldPosition(scale));
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            bestOption = opt;
                        }
                    }
                    // Debug.Log($"Chọn neighbor gần nhất: ({bestOption.x},{bestOption.y},{bestOption.z})");
                    return bestOption;
                }
            }
            else
            {
                Debug.LogWarning($"A* KHÔNG tìm được đường từ ({current.x},{current.y},{current.z}) tới ({targetCell.x},{targetCell.y},{targetCell.z})");
            }
        }

        // ƯU TIÊN 2: Tránh quay lại previousCell nếu có lựa chọn khác
        var filteredOptions = new List<Cell>(options);
        if (previousCell != null && filteredOptions.Contains(previousCell) && filteredOptions.Count > 1)
        {
            // Debug.Log("uu tiên 2: tránh quay lại previousCell");
            filteredOptions.Remove(previousCell);
        }

        // ƯU TIÊN 3: Ưu tiên cell chưa đi thăm
        foreach (var cell in filteredOptions)
        {
            if (!visitedCells.Contains(cell))
            {
                // Debug.Log("uu tiên 3: chọn cell chưa thăm");
                return cell;
            }
        }

        // ƯU TIÊN 4: Chọn cell có neighbor chưa khám phá
        foreach (var cell in filteredOptions)
        {
            var cellNeighbors = MazeTools.GetNeighborsByCondition(cell, MazeGenerator.MazeGrid, mazeSO.BoxSize, null);
            foreach (var neighbor in cellNeighbors)
            {
                if (!knownCells.Contains(neighbor) && !MazeTools.HasWallBetween(cell, neighbor))
                {
                    // Debug.Log("uu tiên 4: chọn cell có neighbor chưa khám phá");
                    return cell;
                }
            }
        }

        // ƯU TIÊN 5: Tất cả đã thăm và không có frontier → chọn ngẫu nhiên
        return filteredOptions[Random.Range(0, filteredOptions.Count)];
    }

    private void StopAI(float duration = 0f)
    {
        AiEnabled = false;
        animator.SetFloat("Speed", 0f);
        velocity = Vector3.zero;
        
        // Nếu duration > 0, tạm dừng và bật lại sau khoảng thời gian
        if (duration > 0f)
        {
            CancelInvoke(nameof(ResumeAI));
            Invoke(nameof(ResumeAI), duration);
        }
    }

    private void ResumeAI()
    {
        AiEnabled = true;
    }

    public void ResetAIState(Vector3 position)
    {
        this.controller.enabled = false;
        finishTime = UIInformation.timePlay;
        totalTime += finishTime;
        mazeSO = GameManager.Instance._mazeSO;
        target = GameManager.Instance.GoalObj.transform;
        controller ??= GetComponent<CharacterController>();
        scale = GameManager.Instance._mazeSO.GetSizeScale;
        
        // SET position từ parameter (spawn point)
        transform.position = position;
        
        knownCells.Clear();
        visitedCells.Clear();
        currentTargetCell = null;
        previousCell = null;
        targetCell = null;
        nextWaypoint = transform.position;
        hasReachedCurrentTarget = true;
        velocity = Vector3.zero;
        finishTime = 0f;
        totalTime = 0f;
        isFinished = false;
        hasRevealedGoal = false;
        
        // Discover start cell từ vị trí mới
        var startCell = MazeTools.GetCellFromGameObject(gameObject, MazeGenerator.MazeGrid, mazeSO.BoxSize, scale);
        if (startCell == null)
        {
            startCell = MazeGenerator.MazeGrid[0, 0, 0];
        }

        if (startCell != null)
        {
            // Chỉ khám phá, không set position lại
            DiscoverCell(startCell);
            visitedCells.Add(startCell);
            currentTargetCell = startCell;
            nextWaypoint = startCell.GetWorldPosition(scale);
        }

        targetCell = target != null
            ? MazeTools.GetCellFromGameObject(target.gameObject, MazeGenerator.MazeGrid, mazeSO.BoxSize, scale)
            : null;
        
        // Bắt đầu lại AI sau khi reset xong
        this.controller.enabled = true;
        finishTime = 0f;
        AiEnabled = true;
    }

    void MoveTowardWaypoint()
    {
        Vector3 toWaypoint = nextWaypoint - transform.position;
        Vector3 planar = new Vector3(toWaypoint.x, 0f, toWaypoint.z);

        // Di chuyển đến waypoint
        if (planar.sqrMagnitude > arriveRadius * arriveRadius)
        {
            float speed = planar.magnitude > 0 ? 1f : 0f;
            animator.SetFloat("Speed", speed);

            Vector3 dir = planar.normalized;
            controller.Move(dir * playerSO.moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(dir);
        }
        

        // Gravity like CharacterMovement
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += playerSO.gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
