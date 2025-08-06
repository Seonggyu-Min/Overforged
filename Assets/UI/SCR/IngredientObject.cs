using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class IngredientObject : MonoBehaviour
    {
        [SerializeField] private Image ingredientImage;
        [SerializeField] private Image howToMakeImage;
        [SerializeField] private GameObject howToMakeObj;
        [SerializeField] private TMP_Text howToMakeNum;
        [SerializeField] private ImageList imageList;

        public void SetIngredientObject(MaterialType materialType, WoodType wood, OreType ore, Sprite howToMake, string num = "0")
        {
            SetSprite(materialType, wood, ore);
            if (howToMake == null) howToMakeObj.SetActive(false);
            else howToMakeObj.SetActive(true);
            howToMakeImage.sprite = howToMake;
            howToMakeNum.text = num;
        }

        private void SetSprite(MaterialType materialType, WoodType wood, OreType ore)
        {
            if (materialType == MaterialType.Metallic)
            {
                if (ore == OreType.Copper)
                    ingredientImage.sprite = imageList.sprites[0];
                else if (ore == OreType.Steel)
                    ingredientImage.sprite = imageList.sprites[1];
                else if (ore == OreType.Gold)
                    ingredientImage.sprite = imageList.sprites[2];

            }
            else if (materialType == MaterialType.Wooden)
            {
                if (wood == WoodType.Oak)
                    ingredientImage.sprite = imageList.sprites[3];
                if (wood == WoodType.Birch)
                    ingredientImage.sprite = imageList.sprites[4];
            }
            else
            {
                ingredientImage.sprite = imageList.sprites[5];
            }
        }
    }
}