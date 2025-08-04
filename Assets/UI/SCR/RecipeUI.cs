using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class RecipeUI : MonoBehaviour
    {
        // 완성품 이미지
        [SerializeField] private Image outputImage;
        [SerializeField] private Slider leftTime;
        // 재료 이미지들 (프리셋 이미지 슬롯들)
        [SerializeField] private List<IngredientObject> ingredientObjects;

        // 딕셔너리, 완성품 이미지들
        [SerializeField] private ProductSprites productSprites;

        // 현재 이 UI가 표시하고 있는 레시피 데이터
        public JJY.RecipeData curRecipe { get; private set; }
        public int uniqueID;
        private string recipeNameText;
        public CraftData curCraft;
        public WoodType curWood;
        public OreType curOre;
        public MaterialData matData;

        public ProductItemData curProduct => curCraft.ProductItemData;

        // 외부에서 레시피 데이터 받아 UI 설정
        public void Setup(CraftData craftdata, WoodType wood, OreType ore, int id)
        {
            curCraft = craftdata;
            curWood = wood;
            curOre = ore;
            uniqueID = id;

            recipeNameText = $"{matData.oreName[curOre]}{matData.woodName[curWood]}{curProduct.Name}";

            outputImage.sprite = productSprites.Dict[recipeNameText];

            for (int i = 0; i < ingredientObjects.Count; i++)
            {

                if (i < curCraft.Materials.Length)
                {
                    ingredientObjects[i].gameObject.SetActive(true);
                    ingredientObjects[i].SetIngredientObject(curCraft.Materials[i].Image,
                    curCraft.Materials[i].guideImage, curCraft.Materials[i].guideMessage);
                }
                else
                {
                    ingredientObjects[i].gameObject.SetActive(false);
                }
            }
        }

        public void LeftTime(float Values)
        {
            leftTime.value = Values;
        }
    }
}

