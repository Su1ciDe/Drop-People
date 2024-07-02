using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerFreeMove : MoveBase
{
    private CharacterController _controller = null;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    protected override void Move()
    {
        var movementVector = new Vector3(direction.x * moveData.horizontalMovementSpeed, 0, 
            direction.y * moveData.verticalMovementSpeed * multiplier) * Time.deltaTime;

        _controller.Move(movementVector);
    }
    
    protected override void Rotate()
    {
        if(direction == Vector2.zero) return;

        // Handle Rotation
        var currRotation = transform.rotation;
        transform.rotation = Quaternion.Lerp(currRotation, ValidateRotation(), Time.deltaTime * moveData.rotationSpeed);
    }

    private Quaternion ValidateRotation()
    {
        var targetRotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(direction.x, direction.y) * 180 / Mathf.PI, 0));
    
        var rotation = targetRotation.eulerAngles;
    
        rotation.y = rotation.y < 180 ? rotation.y : rotation.y - 360;
    
        targetRotation.eulerAngles = rotation;
    
        return targetRotation;
    }

    private void Update()
    {
        if (CanMove())
        {
            Move();
            
            if(moveData.useRotation)
                Rotate();
        }
    }
}