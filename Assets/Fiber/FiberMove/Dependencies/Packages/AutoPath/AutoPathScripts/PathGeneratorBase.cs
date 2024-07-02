using PathCreation;
using UnityEngine;

public abstract class PathGeneratorBase : MonoBehaviour
{
    public float pathLength = 100f;

    [Header("Start and Finish Prefabs")]
    public GameObject startPrefab;
    public GameObject finishPrefab;

    public PathCreator pathCreator;

    public abstract void GeneratePath();

    public virtual void SpawnStart()
    {
    }

    public virtual void SpawnFinish()
    {
    }

    public virtual Vector3 GetStartPoint() 
    {
        return pathCreator.path.GetPointAtDistance(0, EndOfPathInstruction.Stop);
    }

    public virtual Quaternion GetStartRotation()
    {
        return pathCreator.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop);
    }

    public virtual Vector3 GetEndPoint()
    {
        return pathCreator.path.GetPointAtDistance(pathLength, EndOfPathInstruction.Stop);
    }

    public virtual Quaternion GetEndRotation()
    {
        return pathCreator.path.GetRotationAtDistance(pathLength, EndOfPathInstruction.Stop);
    }
}
