using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{

    [SerializeField] List<MeshFilter> meshes;


    public void Combine()
    {
        var combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;
            
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
    }
}
