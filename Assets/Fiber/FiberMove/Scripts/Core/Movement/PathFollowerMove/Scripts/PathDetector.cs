using PathCreation;
using UnityEngine;

public class PathDetector : MonoBehaviour
{
    private PathFollower _path = null;

    private void Awake()
    {
        _path = GetComponentInParent<PathFollower>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position + transform.forward, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            var path = hit.collider.GetComponentInParent<PathCreator>();
        
            if (path != null)
            {
                _path.UpdatePath(path);
            }
        }
    }
}
