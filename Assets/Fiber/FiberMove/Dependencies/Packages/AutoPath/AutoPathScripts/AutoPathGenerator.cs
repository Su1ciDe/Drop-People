using PathCreation;
using PathCreation.Examples;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PathCreator), typeof(RoadMeshCreator))]
public class AutoPathGenerator : PathGeneratorBase
{
    [Header("Path Creation Fields")]
    [SerializeField] int amountOfPoints = 5;
    public float roadWidth = 3f;
    [SerializeField] float xRandomness;
    [SerializeField] float yRandomness;

    private GameObject previousStartPrefab;
    private GameObject previousFinishPrefab;

    public bool useStartAndFinish;

    public override void GeneratePath()
    {
        var roadMeshCreator = GetComponent<RoadMeshCreator>();

        InitializePath();
        roadMeshCreator.textureTiling = pathLength;
        roadMeshCreator.flattenSurface = true;
        roadMeshCreator.roadWidth = roadWidth;
        roadMeshCreator.TriggerUpdate();
    }
    
    private void InitializePath()
    {
        pathCreator = GetComponent<PathCreator>();

        BezierPath bezierPath = new BezierPath(GeneratePoints());
        pathCreator.bezierPath = bezierPath;

        pathCreator.bezierPath.GlobalNormalsAngle = 90f;
        pathCreator.bezierPath.AutoControlLength = 0.3f;

        RemovePreviousStartAndFinish();
        if (useStartAndFinish)
        {
            SpawnStart();
            SpawnFinish();
            
            AdjustRotations();
        }
    }

    public override void SpawnStart()
    {
        #if UNITY_EDITOR
        
        previousStartPrefab = PrefabUtility.InstantiatePrefab(startPrefab, transform) as GameObject;
        previousStartPrefab.transform.position = pathCreator.path.GetPointAtDistance(0, EndOfPathInstruction.Stop);
        previousStartPrefab.transform.rotation = pathCreator.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop);
        
        #endif
    }

    public override void SpawnFinish()
    {
        #if UNITY_EDITOR

        previousFinishPrefab = PrefabUtility.InstantiatePrefab(finishPrefab, transform) as GameObject;
        previousFinishPrefab.transform.position = pathCreator.path.GetPointAtDistance(pathCreator.path.length, EndOfPathInstruction.Stop);
        previousFinishPrefab.transform.rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.length, EndOfPathInstruction.Stop);
        
        #endif
    }

    private Vector3[] GeneratePoints()
    {
        var generatedPoints = new Vector3[amountOfPoints];

        generatedPoints[0] = Vector3.zero;

        float distanceBetweenPoints = pathLength / (amountOfPoints-1);

        for (int i = 1; i < generatedPoints.Length; i++)
        {
            var randX = Random.Range(-xRandomness, xRandomness);
            var randY = Random.Range(-yRandomness, yRandomness);

            generatedPoints[i] = new Vector3(randX, randY, distanceBetweenPoints * i);
        }

        return generatedPoints;
    }

    public void UpdateStartAndFinish()
    {
        var pathCreator = GetComponent<PathCreator>();

        if (previousStartPrefab != null && previousFinishPrefab != null)
        {
            previousStartPrefab.transform.position = pathCreator.path.GetPointAtDistance(0, EndOfPathInstruction.Stop);
            previousStartPrefab.transform.rotation = pathCreator.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop);
            previousFinishPrefab.transform.position = pathCreator.path.GetPointAtDistance(pathCreator.path.length, EndOfPathInstruction.Stop);
            previousFinishPrefab.transform.rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.length, EndOfPathInstruction.Stop);

            AdjustRotations();
        }
    }

    private void AdjustRotations()
    {
        var finishPrefabRotation = previousFinishPrefab.transform.eulerAngles;
        previousFinishPrefab.transform.eulerAngles = new Vector3(finishPrefabRotation.x, finishPrefabRotation.y, 0);
            
        var startPrefabRotation = previousStartPrefab.transform.eulerAngles;
        previousStartPrefab.transform.eulerAngles = new Vector3(startPrefabRotation.x, startPrefabRotation.y, 0);
    }

    public void RemovePreviousStartAndFinish()
    {
        if (previousStartPrefab != null && previousFinishPrefab != null)
        {
            DestroyImmediate(previousStartPrefab, true);
            DestroyImmediate(previousFinishPrefab, true);
        }
    }
}
