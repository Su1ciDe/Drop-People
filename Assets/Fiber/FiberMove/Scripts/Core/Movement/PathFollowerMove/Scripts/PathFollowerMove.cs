using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(PathDetector))]
public class PathFollowerMove : MoveBase
{
    [Header("@References")]
    [SerializeField] private Transform playerModel = null;
    
    private Vector3 _rotationVelocity = Vector3.zero;
    
    private Vector3 _xDirection = Vector3.zero;

    protected override void Move()
    {
        var localPosition = transform.localPosition;
        var horizontal = direction.x * moveData.horizontalMovementSpeed;
        localPosition += new Vector3(horizontal, 0, 0);
        localPosition.x = Mathf.Clamp(localPosition.x, -positionLimit, positionLimit);
        transform.localPosition = localPosition;
    }

    protected override void Rotate()
    {
        var horizontal = direction.x * moveData.horizontalMovementSpeed;
        var dir = new Vector3(horizontal, playerModel.localPosition.y, moveData.verticalMovementSpeed * Time.deltaTime);
        dir = Vector3.SmoothDamp(_xDirection, dir, ref _rotationVelocity, 0.2F);
        _xDirection = dir;

        var currentRot = playerModel.localRotation;
        var targetRot = Quaternion.LookRotation(dir);
        playerModel.localRotation = Quaternion.Lerp(currentRot, targetRot, Time.deltaTime * moveData.rotationSpeed);
        
        var ry = playerModel.localEulerAngles.y;
        if (ry >= 180) ry -= 360;
        playerModel.localEulerAngles = new Vector3(0, Mathf.Clamp(ry, -rotationLimit,rotationLimit), 0);
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