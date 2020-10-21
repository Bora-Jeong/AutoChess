using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMeshCollider : MonoBehaviour
{
    private Mesh mesh;
    private SkinnedMeshRenderer skinMesh;
    private MeshCollider colider;

    private void Start()
    {
        skinMesh = GetComponent<SkinnedMeshRenderer>();
        mesh = new Mesh();
        colider = GetComponent<MeshCollider>();
    }


    private void Update()
    {
        skinMesh.BakeMesh(mesh);
        colider.sharedMesh = mesh;
    }
}
