using UnityEngine;

public interface IMoveAble 
{
    bool _isCanMove { get; set; }
    void HandleMove(float direction);
    void SetFacingDirection(Vector2 direction);
}
