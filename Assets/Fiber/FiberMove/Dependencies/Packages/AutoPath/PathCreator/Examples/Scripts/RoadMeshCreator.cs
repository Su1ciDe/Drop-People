using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {
        [Header("Road settings")]
        public float roadWidth = 6f;
        [Range(0, 500f)]
        public float thickness = 250f;
        public bool flattenSurface;
        [Range(6, 18)]
        public int bewelSegmentCount = 6;
        [Range(0.25f, 3f)]
        public float bewelRadius = 0.5f;

        [Range(-0.45f, 0)] 
        public float bewelHeight = 0f;

        [Header("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public Material bewelMaterial;
        
        [HideInInspector]
        public float textureTiling = 1;

        public float tilingRate = 10;
        
        [SerializeField, HideInInspector]
        GameObject meshHolder;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        Mesh mesh;
        
        #if UNITY_EDITOR

        private void OnValidate()
        {
            AssignMaterials();
        }

        #endif

        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                CreateRoadMesh();
            }

            if (GetComponent<AutoZigZagPathGenerator>())
            {
                GetComponent<AutoZigZagPathGenerator>().UpdateStartAndFinish();
            }

            else if (GetComponent<AutoPathGenerator>())
            {
                GetComponent<AutoPathGenerator>().UpdateStartAndFinish();
            }
        }

        void CreateRoadMesh()
        {
            List<Vector3> vertList = new List<Vector3>();
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length + 2 * (bewelSegmentCount + 1) * path.NumPoints];
            Vector3[] normals = new Vector3[verts.Length + 2 * (bewelSegmentCount + 1) * path.NumPoints];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];
            List<int> rightSideTriangles = new List<int>();
            List<int> leftSideTriangles = new List<int>();

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

            List<Vector3> rightBewelVerts = new List<Vector3>();
            List<Vector3> leftBewelVerts = new List<Vector3>();
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0] - localRight * (bewelRadius * 2F - Mathf.Abs(bewelHeight)) + localUp * bewelHeight;
                verts[vertIndex + 5] = verts[vertIndex + 1] + localRight * (bewelRadius * 2F - Mathf.Abs(bewelHeight))  + localUp * bewelHeight;
                verts[vertIndex + 6] = verts[vertIndex + 2] - localRight * (bewelRadius * 2F - Mathf.Abs(bewelHeight)) - localUp * thickness;
                verts[vertIndex + 7] = verts[vertIndex + 3] + localRight * (bewelRadius * 2F - Mathf.Abs(bewelHeight))  - localUp * thickness;

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;
                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }
                }

                vertIndex += 8;
                triIndex += 6;
            }

            //rightBewelVerts
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);
                Vector3 rightPoint = vertSideB + localRight * (bewelRadius * 2F - Mathf.Abs(bewelHeight)) + localUp * bewelHeight;
                var center = vertSideB + localRight * (bewelRadius- Mathf.Abs(bewelHeight))  + localUp * bewelHeight;;
                rightBewelVerts.AddRange(GetRightBewelPoints(center, bewelRadius, bewelSegmentCount, localRight, localUp, vertSideB, rightPoint ).ToList());
            }

            //leftBewelVerts
            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
                Vector3 leftPoint = vertSideA - localRight * (bewelRadius * 2F - Mathf.Abs(bewelHeight))+ localUp * bewelHeight;
                var center = vertSideA - localRight * (bewelRadius - Mathf.Abs(bewelHeight)) + localUp * bewelHeight;;
                leftBewelVerts.AddRange(GetLeftBewelPoints(center, bewelRadius, bewelSegmentCount, localRight, localUp, leftPoint, vertSideA ).ToList());
            }

            //rightTris
            for (int i = 0; i < path.NumPoints - 1; i++)
            {
                int rootIndex = (i * (bewelSegmentCount + 1)) + verts.Length;
                int rootIndexNext = ((i + 1) * (bewelSegmentCount + 1)) + verts.Length;
                for (int j = 0; j < bewelSegmentCount; j++)
                {
                    int first = rootIndex + j;
                    int second = rootIndexNext + j;
                    int third = rootIndexNext + j + 1;
                    int fourth = rootIndex + j;
                    int fifth = rootIndexNext + j + 1;
                    int sixth = rootIndex + j + 1;
                    rightSideTriangles.Add(sixth);
                    rightSideTriangles.Add(fifth);
                    rightSideTriangles.Add(fourth);
                    rightSideTriangles.Add(third);
                    rightSideTriangles.Add(second);
                    rightSideTriangles.Add(first);
                }
            }

            //leftTris
            for (int i = 0; i < path.NumPoints - 1; i++)
            {
                int rootIndex = (i * (bewelSegmentCount + 1)) + verts.Length + rightBewelVerts.Count;
                int rootIndexNext = ((i + 1) * (bewelSegmentCount + 1)) + verts.Length + rightBewelVerts.Count;
                for (int j = 0; j < bewelSegmentCount; j++)
                {
                    int first = rootIndex + j;
                    int second = rootIndexNext + j;
                    int third = rootIndexNext + j + 1;
                    int fourth = rootIndex + j;
                    int fifth = rootIndexNext + j + 1;
                    int sixth = rootIndex + j + 1;
                    leftSideTriangles.Add(sixth);
                    leftSideTriangles.Add(fifth);
                    leftSideTriangles.Add(fourth);
                    leftSideTriangles.Add(third);
                    leftSideTriangles.Add(second);
                    leftSideTriangles.Add(first);
                }
            }

            //rightUVs
            for (int i = 0; i < path.NumPoints; i++)
            {
                for (int j = 0; j < bewelSegmentCount+1; j++)
                {
                    uvs[verts.Length + j + ((bewelSegmentCount + 1) * i)] = new Vector2((float)j / bewelSegmentCount, path.times[i]);
                }
            }
            //leftUVs
            for (int i = 0; i < path.NumPoints; i++)
            {
                for (int j = 0; j < bewelSegmentCount+1; j++)
                {
                    uvs[verts.Length + rightBewelVerts.Count + j + ((bewelSegmentCount + 1) * i)] = new Vector2((float)j / bewelSegmentCount, path.times[i]);
                }
            }


            vertList.AddRange(verts.ToList());
            vertList.AddRange(rightBewelVerts);
            vertList.AddRange(leftBewelVerts);

            mesh.Clear();
            mesh.vertices = vertList.ToArray();
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 5;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.SetTriangles(underRoadTriangles, 1);
            mesh.SetTriangles(sideOfRoadTriangles, 2);
            mesh.SetTriangles(rightSideTriangles, 3);
            mesh.SetTriangles(leftSideTriangles, 4);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        Vector3[] GetRightBewelPoints(Vector3 center, float radius, int segmentCount, Vector3 localRight, Vector3 localUp, Vector3 leftPoint, Vector3 rightPoint)
        {
            Vector3[] bewelPoints = new Vector3[segmentCount + 1];
            for (int i = 0; i < bewelPoints.Length; i++)
            {
                var angle = Vector3.Angle((leftPoint - center).normalized, (rightPoint - center).normalized);
                var curAngle = Mathf.Deg2Rad * (((angle / segmentCount) * i));
                bewelPoints[i] = center + localRight * (radius * Mathf.Cos(curAngle)) + localUp * (radius * Mathf.Sin(curAngle));
            }
            return bewelPoints;
        }

        Vector3[] GetLeftBewelPoints(Vector3 center, float radius, int segmentCount, Vector3 localRight, Vector3 localUp, Vector3 leftPoint, Vector3 rightPoint)
        {
            Vector3[] bewelPoints = new Vector3[segmentCount + 1];
            for (int i = 0; i < bewelPoints.Length; i++)
            {
                var angle = Vector3.Angle((leftPoint - center).normalized, (rightPoint - center).normalized);
                var curAngle = Mathf.Deg2Rad * (180 - (angle - ((angle / segmentCount) * i)));
                bewelPoints[i] = center + localRight * (radius * Mathf.Cos(curAngle)) + localUp * (radius * Mathf.Sin(curAngle));
            }
            return bewelPoints;
        }

        //private void OnDrawGizmos()
        //{
        //    foreach(var vert in mesh.vertices)
        //    {
        //        Gizmos.DrawSphere(vert, 0.05f);
        //    }

        //}

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents()
        {

            if (meshHolder == null)
            {
                meshHolder = new GameObject("Road Mesh Holder");
            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;
            meshHolder.transform.parent = transform;

            // Ensure mesh renderer and filter components are assigned
            if (!meshHolder.gameObject.GetComponent<MeshFilter>())
            {
                meshHolder.gameObject.AddComponent<MeshFilter>();
            }
            if (!meshHolder.GetComponent<MeshRenderer>())
            {
                meshHolder.gameObject.AddComponent<MeshRenderer>();
            }

            meshRenderer = meshHolder.GetComponent<MeshRenderer>();
            meshFilter = meshHolder.GetComponent<MeshFilter>();
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            meshFilter.sharedMesh = mesh;

            if (meshHolder.gameObject.GetComponent<MeshCollider>())
            {
                #if UNITY_EDITOR
                    DestroyImmediate(meshHolder.GetComponent<MeshCollider>(), true);
                #else
                    Destroy(meshHolder.GetComponent<MeshCollider>());
                #endif
            }

            meshHolder.AddComponent<MeshCollider>().convex = true;
        }

        void AssignMaterials()
        {
            if (meshRenderer == null) return;
            
            if (roadMaterial != null && undersideMaterial != null)
            {
                roadMaterial.SetTextureScale("_MainTex", 
                    new Vector2(roadMaterial.GetTextureScale("_MainTex").x, textureTiling / tilingRate));
                meshRenderer.sharedMaterials = new Material[] 
                    { roadMaterial, undersideMaterial, undersideMaterial, bewelMaterial, bewelMaterial };
            }
        }
    }
}