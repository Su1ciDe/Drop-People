using PathCreation;
using PathCreation.Examples;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PathCreator), typeof(RoadMeshCreator))]
public class AutoZigZagPathGenerator : MonoBehaviour
{
    [Header("Path Creation Fields")]
    [SerializeField] float pathLenght = 100f;
    [SerializeField] int amountOfPoints = 5;
    public float roadWidth = 3f;
    [SerializeField] float cornerAngle = 90f;

    [Header("UpAndDownRandomness")]
    [SerializeField] bool useY;
    [SerializeField] float yRandomness = 0f;

    [Header("Bevel Related Fields")]
    [SerializeField] int cornerBewelResolution = 10;
    [SerializeField] float cornerBewelRadius = 5;

    [Header("Start and Finish Prefabs")]
    [SerializeField] GameObject startPrefab;
    [SerializeField] GameObject finishPrefab;

    private GameObject previousStartPrefab;
    private GameObject previousFinishPrefab;

    private List<Vector3> beweledPathPoints = new List<Vector3>();

    public bool useStartAndFinish;

    public void GeneratePath()
    {
        var roadMeshCreator = GetComponent<RoadMeshCreator>();

        InitializePath();
        roadMeshCreator.textureTiling = pathLenght;
        roadMeshCreator.flattenSurface = true;
        roadMeshCreator.roadWidth = roadWidth;
    }

    private void InitializePath()
    {
        var pathCreator = GetComponent<PathCreator>();

        BezierPath bezierPath = new BezierPath(PathPointsWithBewel(GeneratePoints()));
        pathCreator.bezierPath = bezierPath;

        pathCreator.bezierPath.GlobalNormalsAngle = 90f;
        pathCreator.bezierPath.AutoControlLength = 0.01f;
        pathCreator.EditorData.vertexPathMaxAngleError = 45F;

        RemovePreviousStartAndFinish();

        //previousStartPrefab = PrefabUtility.InstantiatePrefab(startPrefab, transform) as GameObject;
        //previousFinishPrefab = PrefabUtility.InstantiatePrefab(finishPrefab, transform) as GameObject;

        if (useStartAndFinish)
        {
            previousStartPrefab = Instantiate(startPrefab, transform);
            previousFinishPrefab = Instantiate(finishPrefab, transform);
            
            previousStartPrefab.transform.position = pathCreator.path.GetPointAtDistance(0, EndOfPathInstruction.Stop);
            previousStartPrefab.transform.rotation = pathCreator.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop);

            previousFinishPrefab.transform.position = pathCreator.path.GetPointAtDistance(pathCreator.path.length, EndOfPathInstruction.Stop);
            previousFinishPrefab.transform.rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.length, EndOfPathInstruction.Stop);
            
            AdjustRotations();
        }
    }

    private List<Vector3> PathPointsWithBewel(Vector3[] generatedPoints)
    {
        beweledPathPoints.Clear();
        beweledPathPoints.Add(generatedPoints[0]);

        for (int i = 1; i < generatedPoints.Length - 1; i++)
        {
            for (int j = 0; j < cornerBewelResolution; j++)
            {
                beweledPathPoints.Add(GenerateBewelPoints(generatedPoints[i - 1], generatedPoints[i], generatedPoints[i + 1])[j]);
            }
        }

        beweledPathPoints.Add(generatedPoints[generatedPoints.Length - 1]);
        return beweledPathPoints;
    }

    private Vector3[] GeneratePoints()
    {
        var generatedPoints = new Vector3[amountOfPoints];

        generatedPoints[0] = Vector3.zero;
        
        float distanceBetweenPoints = pathLenght / (amountOfPoints - 1);

        for (int i = 1; i < generatedPoints.Length; i++)
        {
            Vector3 newPointOffset;
            if (i % 2 != 0)
            {
                newPointOffset = new Vector3(0f, 0f, distanceBetweenPoints);
            }
            else
            {
                newPointOffset = new Vector3(distanceBetweenPoints * (Random.Range(0, 2) * 2 - 1), 0f, 0f);
            }

            if (useY)
            {
                newPointOffset.y = Random.Range(-yRandomness, yRandomness);
            }

            generatedPoints[i] = generatedPoints[i - 1] + newPointOffset;
        }
        return generatedPoints;
    }

    private Vector3[] GenerateBewelPoints(Vector3 prevCorner, Vector3 corner, Vector3 nextCorner)
    {
        var generatedBewelPoints = new Vector3[cornerBewelResolution+1];
        //var cornerBewelRadius = roadWidth * 2;

        if (prevCorner.z > corner.z && nextCorner.x > corner.x)
        {
            var center = new Vector3(corner.x + cornerBewelRadius, corner.y, corner.z + cornerBewelRadius);

            var angle = 180;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.x > corner.x && nextCorner.z > corner.z)
        {
            var center = new Vector3(corner.x + cornerBewelRadius, corner.y, corner.z + cornerBewelRadius);

            var angle = 270;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.x > corner.x && nextCorner.z < corner.z)
        {
            var center = new Vector3(corner.x + cornerBewelRadius, corner.y, corner.z - cornerBewelRadius);

            var angle = 90;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.z < corner.z && nextCorner.x > corner.x)
        {
            var center = new Vector3(corner.x + cornerBewelRadius, corner.y, corner.z - cornerBewelRadius);

            var angle = 180;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.x < corner.x && nextCorner.z < corner.z)
        {
            var center = new Vector3(corner.x - cornerBewelRadius, corner.y, corner.z - cornerBewelRadius);

            var angle = 90;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.z < corner.z && nextCorner.x < corner.x)
        {
            var center = new Vector3(corner.x - cornerBewelRadius, corner.y, corner.z - cornerBewelRadius);

            var angle = 0;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.z > corner.z && nextCorner.x < corner.x)
        {
            var center = new Vector3(corner.x - cornerBewelRadius, corner.y, corner.z + cornerBewelRadius);

            var angle = 0;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle - (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }
        else if (prevCorner.x < corner.x && nextCorner.z > corner.z)
        {
            var center = new Vector3(corner.x - cornerBewelRadius, corner.y, corner.z + cornerBewelRadius);

            var angle = 270;

            for (int i = 0; i < cornerBewelResolution + 1; i++)
            {
                generatedBewelPoints[i] = new Vector3(center.x + cornerBewelRadius * Mathf.Cos((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180),
                                                      center.y,
                                                      center.z + cornerBewelRadius * Mathf.Sin((angle + (i * (cornerAngle / cornerBewelResolution))) * Mathf.PI / 180));

            }
        }

        return generatedBewelPoints;
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

    public void RemovePreviousStartAndFinish()
    {
        if (previousStartPrefab != null && previousFinishPrefab != null)
        {
            DestroyImmediate(previousStartPrefab, true);
            DestroyImmediate(previousFinishPrefab, true);
        }
    }
    
    private void AdjustRotations()
    {
        var finishPrefabRotation = previousFinishPrefab.transform.eulerAngles;
        previousFinishPrefab.transform.eulerAngles = new Vector3(finishPrefabRotation.x, finishPrefabRotation.y, 0);
            
        var startPrefabRotation = previousStartPrefab.transform.eulerAngles;
        previousStartPrefab.transform.eulerAngles = new Vector3(startPrefabRotation.x, startPrefabRotation.y, 0);
    }
}
