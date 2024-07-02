using UnityEngine;
using PathCreation;

public class PathFollower : MonoBehaviour
{
    private PathCreator _currentPath = null;

    private MoveBase _movement = null;

    private float _distance = 0F;
    
    private Quaternion _prevRotation = Quaternion.identity;

    private void Awake()
    {
        _movement = GetComponentInChildren<MoveBase>();
    }

    private void Update()
    {
        // !!!!!!!!REFACTOR
        if (_movement.CanMove())
        {
            UpdateDistance();
            
            var currentPos = transform.position;
            var targetPos = _currentPath.path.GetPointAtDistance(_distance, EndOfPathInstruction.Stop);
            transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * _movement.Speed * 10F);
            transform.rotation = Quaternion.Lerp(_prevRotation, _currentPath.path.GetRotationAtDistance(_distance, EndOfPathInstruction.Stop), 
                Time.deltaTime * _movement.RotationSpeed);
            UpdatePrevRotation();
        }
    }

    private void UpdateDistance()
    {
        if (_currentPath != null)
        {
            _distance = _currentPath.path.GetClosestDistanceAlongPath(transform.position + 
                                                                      transform.forward * (Time.deltaTime * _movement.Speed));
        }
    }

    private void UpdatePrevRotation()
    {
        _prevRotation = transform.rotation;
    }

    private void ResetDistance()
    {
        _distance = _currentPath.path.GetClosestDistanceAlongPath(transform.position);
    }

    public void UpdatePath(PathCreator path)
    {
        if (_currentPath != path)
        {
            _currentPath = path;

            ResetDistance();
        }
    }
}