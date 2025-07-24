using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHG;

namespace KMS
{
    public class MaterialItem : Item
    {
        private bool isHot;

        private MeshFilter mesh;
        private MeshRenderer render;

        void Awake()
        {
            mesh = GetComponent<MeshFilter>();
            render = GetComponent<MeshRenderer>();
        }

        protected override void InitItemData(TestItemData itemdata)
        {
            mesh.sharedMesh = data.prefab.GetComponent<MeshFilter>().sharedMesh;
            render.sharedMaterials = data.prefab.GetComponent<MeshRenderer>().sharedMaterials;

        }

        public void ChangeToNext()
        {
            if (data == null) return;
            //Data = Data.next;
        }
    }   
}
