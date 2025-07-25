using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;
using Photon.Pun;

    public class MaterialItem : Item
    {
        private bool isHot;
        private OreType ore;
        public OreType Ore
        {
            get
            {
                return ore;
            }
            set
            {
                ore = value;
                SetMaterial();
            }
        }
        private WoodType wood;
        public WoodType Wood
        {
            get
            {
                return wood;
            }
            set
            {
                wood = value;
                SetMaterial();
            }
        }

        private MeshFilter mesh;
        private MeshRenderer render;

        private Material CurrentOreMat => matCatalog.OreDict[ore];
        private Material CurrentWoodMat => matCatalog.WoodDict[wood];

        private MaterialItemData matData;

    public MaterialVariation Variation => matData.materialVariation;

        void Awake()
        {
            mesh = model.GetComponent<MeshFilter>();
            render = model.GetComponent<MeshRenderer>();
            ore = OreType.None;
            wood = WoodType.None;
        }

        protected override void InitItemData(ItemData itemdata)
        {
            matData = itemdata as MaterialItemData;
            mesh.sharedMesh = data.ItemPrefab.GetComponent<MeshFilter>().sharedMesh;
            render.sharedMaterials = data.ItemPrefab.GetComponent<MeshRenderer>().sharedMaterials;
            SetMaterial();
            if (isHot) render.sharedMaterial = matCatalog.HotMetal;

        }
        [PunRPC]
        public void Heat()
        {
            isHot = true;
            if (matData.materialType == MaterialType.Metallic)
            {
                render.sharedMaterial = matCatalog.HotMetal;

            }
            else if (matData.materialType == MaterialType.Mineral)
            {
                render.sharedMaterials = new Material[] {matCatalog.HotMetal, matCatalog.HotMetal};
            }
        }
        [PunRPC]
        public void Cool()
        {
            isHot = false;
            SetMaterial();
        }
        [PunRPC]
        public void ChangeToNext()
        {
            if (data == null) return;
            Data = matData.nextMaterial;
        }

        private void SetMaterial()
        {
            if (matData.materialType == MaterialType.Metallic)
            {
                render.sharedMaterial = CurrentOreMat;

            }
            else if (matData.materialType == MaterialType.Wooden)
            {
                render.sharedMaterial = CurrentWoodMat;

            }
            else if (matData.materialType == MaterialType.Mineral)
            {
                render.sharedMaterials = new Material[] {matCatalog.Stone, CurrentOreMat};
            }
            
        }
    }   
