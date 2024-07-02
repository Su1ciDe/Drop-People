using UnityEngine;

public class MoveBase : MonoBehaviour
{
    [SerializeField] protected MoveData moveData = null;

    [Header("@Configurations")]
    [SerializeField] protected float positionLimit = 4F;

    [SerializeField] protected float rotationLimit = 45F;
    
    protected float multiplier = 1;

    protected bool shouldMove = false;
    
    protected Vector2 direction = Vector2.zero;

    public float Speed => moveData.verticalMovementSpeed * multiplier;

    public float RotationSpeed => moveData.rotationSpeed;

    // Control character's movement (forward or backward)
    // If rotationLimit is bigger than 90F, the character can rotate backward.
    // So character can move backward.
    private bool IsUsingMultiDirection()
    {
        return rotationLimit > 90F;
    }

    protected float GetVerticalSign()
    {
        return IsUsingMultiDirection() ? Mathf.Sign(direction.y) : 1;
    }
    
    private void ShouldMove(bool state)
    {
        shouldMove = state;
    }

    public void StartMovement()
    {
        ShouldMove(true);
    }

    public void StopMovement()
    {
        ShouldMove(false);
    }

    private void UpdateDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }
    
    public void ChangeSpeed(float percent)
    {
        multiplier = percent;
    }
    
    public bool CanMove()
    {
        return (_isPressed || moveData.constantForward) && shouldMove;
    }
    
    protected virtual void Move()
    {
        Debug.Log("Base Movement");
    }

    protected virtual void Rotate()
    {
        Debug.Log("Base Rotation");
    }

    #region InputSystem

    private bool _isPressed = false;
       
    private void OnPressed()
    {
        ShouldPress(true);
    }

    private void OnReleased()
    {
        ShouldPress(false);
        
        if(!IsUsingMultiDirection())
            UpdateDirection(Vector2.zero);
    }

    private void OnDrag(Vector2 newDirection)
    {
        UpdateDirection(newDirection);
    }
        
    private void ShouldPress(bool state)
    {
        _isPressed = state;
    }

    #endregion
    
    private void OnEnable()
    {
        InputBase.OnInputPressed += OnPressed;
        InputBase.OnInputReleased += OnReleased;
        InputBase.OnInputDrag += OnDrag;
    }

    private void OnDisable()
    {
        InputBase.OnInputPressed -= OnPressed;
        InputBase.OnInputReleased -= OnReleased;
        InputBase.OnInputDrag -= OnDrag;
    }
}