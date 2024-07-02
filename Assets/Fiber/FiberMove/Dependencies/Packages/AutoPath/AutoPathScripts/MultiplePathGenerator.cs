using System.Collections.Generic;
using UnityEngine;

public class MultiplePathGenerator : MonoBehaviour
{
    [SerializeField] int amountOfPathsToGenerate = 3;

    [SerializeField] List<GameObject> middleParts = new List<GameObject>();

    [SerializeField] bool useZigZagPath;

    private List<GameObject> pathHolders = new List<GameObject>();

    private List<GameObject> arrangedPathPieces = new List<GameObject>();

    [SerializeField] GameObject startPreFab;
    [SerializeField] GameObject finishPreFab;

    public void GenerateMultiPath()
    {
        ClearPath();
        ArrangePath();
    }

    private void GenerateCurvedPaths()
    {
        for (int i = 0; i < amountOfPathsToGenerate; i++)
        {
            GameObject pathHolder = new GameObject("Path Holder " + i);
            pathHolder.transform.parent = transform;
            if (useZigZagPath)
            {
                var pathGenerator = pathHolder.AddComponent<AutoZigZagPathGenerator>();
                pathGenerator.GeneratePath();
            }
            else
            {
                var pathGenerator = pathHolder.AddComponent<AutoPathGenerator>();
                pathGenerator.GeneratePath();
            }
            pathHolders.Add(pathHolder);
        }
    }

    private void ArrangePath()
    {
        GenerateCurvedPaths();
        pathHolders[0].transform.position = transform.position;
        var pathGen = pathHolders[0].GetComponent<PathGeneratorBase>();
        pathGen.startPrefab = startPreFab;
        pathGen.SpawnStart();
        arrangedPathPieces.Add(pathHolders[0]);

        
        arrangedPathPieces.Add(SpawnedMiddlePart(pathGen.GetEndPoint(), pathGen.GetEndRotation()));

        for (int i = 1; i < pathHolders.Count-1; i++)
        {
            pathHolders[i].transform.position = arrangedPathPieces[arrangedPathPieces.Count - 1].GetComponent<MiddlePartAnchors>().EndAnchor;
            pathHolders[i].transform.rotation = arrangedPathPieces[arrangedPathPieces.Count - 1].GetComponent<MiddlePartAnchors>().EndAnchorRotation;
            arrangedPathPieces.Add(pathHolders[i]);
            arrangedPathPieces.Add(SpawnedMiddlePart(pathHolders[i].GetComponent<PathGeneratorBase>().GetEndPoint(), pathHolders[i].GetComponent<PathGeneratorBase>().GetEndRotation()));
        }

        pathHolders[pathHolders.Count - 1].transform.position = arrangedPathPieces[arrangedPathPieces.Count - 1].GetComponent<MiddlePartAnchors>().EndAnchor;
        pathHolders[pathHolders.Count - 1].transform.rotation = arrangedPathPieces[arrangedPathPieces.Count - 1].GetComponent<MiddlePartAnchors>().EndAnchorRotation;
        var endPathGen = pathHolders[pathHolders.Count - 1].GetComponent<PathGeneratorBase>();
        endPathGen.finishPrefab = finishPreFab;
        endPathGen.SpawnFinish();
        arrangedPathPieces.Add(pathHolders[pathHolders.Count - 1]);
    } 

    private GameObject GetRandomMiddlePart()
    {
        var rand = Random.Range(0, middleParts.Count);
        return middleParts[rand];
    }

    private GameObject SpawnedMiddlePart(Vector3 position, Quaternion rotation)
    {
        return Instantiate(GetRandomMiddlePart(), position, rotation, transform);
    }

    private void ClearPath()
    {
        if(arrangedPathPieces.Count > 0)
        {
            foreach (var item in arrangedPathPieces)
            {
                DestroyImmediate(item);
            }
            pathHolders.Clear();
            arrangedPathPieces.Clear();
        }
    }
}
