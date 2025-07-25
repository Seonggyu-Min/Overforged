using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;
using Unity.VisualScripting.Antlr3.Runtime;

    public class ProductItem : Item
    {
        private ProductItemData productData;

        private MeshRenderer[] render;

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
                SetMetalicMaterial();
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
                SetWoodenMaterial();
            }
        }

        private Material CurrentOreMat => matCatalog.OreDict[ore];
        private Material CurrentWoodMat => matCatalog.WoodDict[wood];
        protected override void InitItemData(ItemData itemdata)
        {

            productData = itemdata as ProductItemData;
            Instantiate(productData.ItemPrefab, model.transform);
            render = model.GetComponentsInChildren<MeshRenderer>();
        }

        private void SetMetalicMaterial()
        {
            switch (productData.productType)
            {
                case ProductType.Sword:
                    render[0].sharedMaterial = CurrentOreMat;
                    break;
                case ProductType.Axe:
                    render[0].sharedMaterial = CurrentOreMat;
                    break;
                case ProductType.Hammer:
                    render[2].sharedMaterial = CurrentOreMat;
                    break;
            }

        }
        private void SetWoodenMaterial()
        {
            switch (productData.productType)
            {
                case ProductType.Sword:
                    render[1].sharedMaterial = CurrentWoodMat;
                    render[3].sharedMaterial = CurrentWoodMat;
                    break;
                case ProductType.Axe:
                    render[2].sharedMaterial = CurrentWoodMat;
                    break;
                case ProductType.Hammer:
                    render[1].sharedMaterial = CurrentWoodMat;
                    break;
                case ProductType.Bow:
                    render[0].sharedMaterial = CurrentWoodMat;
                    break;
            }
        }
    }
