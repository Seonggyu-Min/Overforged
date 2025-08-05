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

        public void SetIngredientObject(WoodType wood, OreType ore, Sprite howToMake, string num = "0")
        {
            SetSprite(wood, ore);
            if (howToMake == null) howToMakeObj.SetActive(false);
            else howToMakeObj.SetActive(true);
            howToMakeImage.sprite = howToMake;
            howToMakeNum.text = num;
        }

        private void SetSprite(WoodType wood, OreType ore)
        {
            if (wood == WoodType.None)
            {
                if (ore == OreType.Copper)
                    ingredientImage.sprite = imageList.sprites[0];
                else if (ore == OreType.Steel)
                    ingredientImage.sprite = imageList.sprites[1];
                else if (ore == OreType.Gold)
                    ingredientImage.sprite = imageList.sprites[2];
                else if (ore == OreType.None)
                    ingredientImage.sprite = imageList.sprites[5];
            }
            else
            {
                if (wood == WoodType.Oak)
                    ingredientImage.sprite = imageList.sprites[3];
                if (wood == WoodType.Birch)
                    ingredientImage.sprite = imageList.sprites[4];
            }
        }
    }
}