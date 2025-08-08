using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;
using SCR;
using System;

namespace SHG
{
    public class BotSceneRecipeUI : MonoBehaviour
    {
        [SerializeField] [Required]
        ProductSprites sprites;
        [SerializeField]
        [Required]
        Image productImage;

        static bool isLoaded;

        public void SetUp(CraftData craftData, WoodType woodType, OreType oreType)
        {
            if (!isLoaded) {
                this.sprites.Init();
                isLoaded = true;
            }
            string wood = woodType.ToString();
            string ore = oreType.ToString();
            string key = $"{ore} {wood} {craftData.ProductItemData.productType}";
            if (this.sprites.Dict.TryGetValue( key, out Sprite sprite))
            {
                this.productImage.sprite = sprite;
            }
        }
    }
}