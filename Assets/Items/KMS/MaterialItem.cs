using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;

namespace KMS
{
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

        private MaterialItemData matData;

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

        }

        public void Heat()
        {
            isHot = true;
            render.sharedMaterial = matCatalog.HotMetal;
        }
        public void Cool()
        {
            isHot = false;
            SetMaterial();
        }

        public void ChangeToNext()
        {
            if (data == null) return;
            Data = matData.nextMaterial;
        }

        private void SetMaterial()
        {
            if (matData.materialType == MaterialType.Metallic)
            {
                switch (ore)
                {
                    case OreType.Steel:
                        render.sharedMaterial = matCatalog.Steel;
                        break;
                    case OreType.Copper:
                        render.sharedMaterial = matCatalog.Copper;
                        break;
                    case OreType.Gold:
                        render.sharedMaterial = matCatalog.Gold;
                        break;
                }

            }
            else if (matData.materialType == MaterialType.Wooden)
            {
                switch (wood)
                {
                    case WoodType.Oak:
                        render.sharedMaterial = matCatalog.Wood;
                        break;
                    case WoodType.Birch:
                        render.sharedMaterial = matCatalog.Birch;
                        break;
                }
            }
        }
    }   
}
