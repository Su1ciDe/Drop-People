using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMove : MoveBase
{
    [Header("@References")]
    [SerializeField] private Transform playerModel = null;
    
    private CharacterController _controller = null;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    protected override void Move()
    {
        var movementVector = new Vector3(direction.x * moveData.horizontalMovementSpeed, 0, 
            moveData.verticalMovementSpeed * multiplier * GetVerticalSign()) * Time.deltaTime;

        _controller.Move(movementVector);

        ValidateMovement();
    }
    
    protected override void Rotate()
    {
        // Handle Rotation
        var currRotation = playerModel.rotation;
        
        playerModel.rotation = Quaternion.Lerp(currRotation, ValidateRotation(), Time.deltaTime * moveData.rotationSpeed);
    }

    private void ValidateMovement()
    {
        var pos = transform.position;
        
        pos.x = Mathf.Clamp(pos.x, -positionLimit, positionLimit);
        
        transform.position = pos;
    }
    
    private Quaternion ValidateRotation()
    {
        var targetRotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(direction.x, direction.y) * 180 / Mathf.PI, 0));
    
        var rotation = targetRotation.eulerAngles;
    
        rotation.y = rotation.y < 180 ? rotation.y : rotation.y - 360;
    
        rotation.y = rotation.y > rotationLimit ? rotationLimit : rotation.y;
    
        rotation.y = rotation.y < -rotationLimit ? -rotationLimit : rotation.y;
            
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

        else
            Rotate();
    }
}