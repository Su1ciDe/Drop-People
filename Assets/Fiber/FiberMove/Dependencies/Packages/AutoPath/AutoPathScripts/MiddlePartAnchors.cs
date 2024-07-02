using UnityEngine;

public class MiddlePartAnchors : MonoBehaviour
{
    [SerializeField] Transform endAnchorTransform;

    public Vector3 StartAnchor { get { return transform.position; } }
    public Vector3 EndAnchor { get { return endAnchorTransform.position; } }
    public Quaternion EndAnchorRotation { get { return endAnchorTransform.rotation; } }
    }
