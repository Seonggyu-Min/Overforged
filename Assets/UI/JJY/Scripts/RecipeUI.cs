using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJY
{
    public class RecipeUI : MonoBehaviour
    {
        // 완성품 이미지
        [SerializeField] private Image outputImage;

        // 레시피 이름 텍스트
        [SerializeField] private TextMeshProUGUI recipeNameText;

        // 재료 이미지들 (프리셋 이미지 슬롯들)
        [SerializeField] private List<Image> ingredientImages;

        // 현재 이 UI가 표시하고 있는 레시피 데이터
        public RecipeData curRecipe { get; private set; }
        public int uniqueID;

        public ProductItemData curProduct;
        public WoodType curWood;
        public OreType curOre;
        public MaterialData matData;

        // 외부에서 레시피 데이터 받아 UI 설정
        public void Setup(ProductItemData itemdata, WoodType wood, OreType ore, int id)
        {
            RecipeData recipeData = null; // 컴파일 에러 제거용 코드
            //curRecipe = recipeData;
            curProduct = itemdata;
            curWood = wood;
            curOre = ore;
            uniqueID = id;

            outputImage.sprite = curProduct.Image;

            recipeNameText.text = $"{matData.oreName[curOre]}{matData.woodName[curWood]}{curProduct.Name}";

            // ingredientImages는 고정된 슬롯이고,
            // 실제 레시피에 따라 일부만 활성화하거나 끔
            for (int i = 0; i < ingredientImages.Count; i++)
            {
                // if (i < recipeData.ingredientImages.Count)
                // {
                //     ingredientImages[i].gameObject.SetActive(true);
                //     ingredientImages[i].sprite = recipeData.ingredientImages[i];
                // }
                // else
                // {
                //     ingredientImages[i].gameObject.SetActive(false);
                // }

                if (i < curProduct.MaterialList.Count)
                {
                    ingredientImages[i].gameObject.SetActive(true);
                    ingredientImages[i].sprite = curProduct.MaterialList[i].Image;
                    // i번째 이미지의 색은 matType마다 색이 바뀌어야함.
                    // i번째칸 이미지가 어떤 재료인지 비교.
                }
                else
                {
                    ingredientImages[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
