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

        public void SetIngredientObject(Sprite ingredient, Sprite howToMake, string num = "0")
        {
            ingredientImage.sprite = ingredient;
            if (howToMake == null) howToMakeObj.SetActive(false);
            else howToMakeObj.SetActive(true);
            howToMakeImage.sprite = howToMake;
            howToMakeNum.text = num;
        }
    }
}