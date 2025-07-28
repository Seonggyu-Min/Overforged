using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJY
{
    [CreateAssetMenu(menuName = "Recipe")]
    public class RecipeData : ScriptableObject
    {
        public string recipeName; // 결과물 이름
        public Sprite outputImage; // 결과물 이미지
        public List<Sprite> ingredientImages; // 재료 이미지 목록
    }
}
