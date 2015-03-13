/*
  ArcMesh.cs
  Mission: Invasion
  Created by Rohun Banerji on March 12, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArcMesh : MonoBehaviour 
{
    public void BuildArc(float radius, float arcAngle, int segments, Material mat)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
#if FULL_DEBUG
        if(!meshFilter)
        {
            Debug.LogError("No Mesh Filter found");
            return;
        }
#endif
        
        MeshCollider meshCol = GetComponent<MeshCollider>();
#if FULL_DEBUG
        if(!meshCol)
        {
            Debug.LogError("No Mesh collider found");
            return;
        }
#endif
        meshFilter.mesh = new Mesh();
        Mesh meshFilterMesh = meshFilter.mesh;
        meshFilterMesh.Clear();
        meshCol.sharedMesh = new Mesh();
        Mesh meshColMesh = meshCol.sharedMesh;
        meshColMesh.Clear();

        float stepAngle = arcAngle * Mathf.Deg2Rad / segments;
        float currAngle = -arcAngle * Mathf.Deg2Rad * 0.5f;
        segments++;

        Vector3[] vertices = new Vector3[segments + 1];
        Vector3[] normals = new Vector3[segments + 1];
        Vector2[] uvs = new Vector2[segments + 1];
        //center
        vertices[0] = Vector3.zero;

        for (int i = 1; i < segments+1; i++)
        {
            vertices[i] = new Vector3(Mathf.Sin(currAngle) * radius, Mathf.Cos(currAngle) * radius, 0.0f);
            normals[i] = Vector3.up;
            uvs[i] = Vector2.zero;
            currAngle += stepAngle;
        }

        int index = 1;
        int numIndices = segments * 3;
        int[] tris = new int[numIndices];
        for (int i = 0; i < numIndices; i+=3)
        {
            tris[i] = 0;//center
            tris[i + 1] = index;
            //last tri
            if(i>=numIndices-3)
            {
                tris[i + 2] = 1;
            }
            else
            {
                tris[i + 2] = index+1;
            }
            index++;
        }
        
        meshFilterMesh.vertices = vertices;
        meshFilterMesh.normals = normals;
        meshFilterMesh.uv = uvs;
        meshFilterMesh.triangles = tris;
        meshColMesh.vertices = vertices;
        meshColMesh.triangles = tris;
        renderer.sharedMaterial = mat;
        meshFilterMesh.RecalculateNormals();
        meshFilterMesh.RecalculateBounds();
        meshFilterMesh.Optimize();
    }
}
