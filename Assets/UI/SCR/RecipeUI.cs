using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class RecipeUI : MonoBehaviourPun
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

        private string recipeNameText;
        private float fullTime;
        private int fullScore;
        private int currentScore;
        private Coroutine startTime;
        public CraftData curCraft;
        public WoodType curWood;
        public OreType curOre;
        public MaterialData matData;

        public ProductItemData curProduct => curCraft.ProductItemData;

        // 외부에서 레시피 데이터 받아 UI 설정
        public void Setup(CraftData craftdata, WoodType wood, OreType ore)
        {
            curCraft = craftdata;
            curWood = wood;
            curOre = ore;
            fullTime = curCraft.Materials.Length * 20f;
            fullScore = curCraft.Materials.Length * 50;
            recipeNameText = $"{matData.oreName[curOre]}{matData.woodName[curWood]}{curProduct.Name}";

            outputImage.sprite = productSprites.Dict[recipeNameText];

            for (int i = 0; i < ingredientObjects.Count; i++)
            {

                if (i < curCraft.Materials.Length)
                {
                    ingredientObjects[i].gameObject.SetActive(true);
                    ingredientObjects[i].SetIngredientObject(curCraft.Materials[i].materialType, wood, ore,
                                        curCraft.Materials[i].guideImage, curCraft.Materials[i].guideMessage);
                }
                else
                {
                    ingredientObjects[i].gameObject.SetActive(false);
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                startTime = StartCoroutine(StartTime());
            }
        }

        public bool CheckRecipe(ProductItemData data, OreType ore, WoodType wood)
        {
            if (curProduct == data && curOre == ore && curWood == wood)
                return true;
            else
                return false;
        }

        private IEnumerator StartTime()
        {
            gameObject.SetActive(true);
            float leftTime = fullTime;
            while (leftTime > 0)
            {
                photonView.RPC("RecipeTime", RpcTarget.All, leftTime, fullTime);
                yield return new WaitForFixedUpdate();
                leftTime -= Time.deltaTime;
            }
            StopCoroutine(startTime);
            photonView.RPC("SyncFalse", RpcTarget.All);
        }

        [PunRPC]
        public void RecipeTime(float leftTime, float fullTime)
        {
            LeftTime(leftTime / fullTime);
            SetScore(leftTime);
        }

        [PunRPC]
        public void SyncFalse()
        {
            gameObject.SetActive(false);
        }

        private void SetScore(float time)
        {
            if (time < fullTime / 3) currentScore = fullScore / 3;
            else if (time < 2 * fullTime / 3) currentScore = 2 * fullScore / 3;
            else currentScore = fullScore;
        }

        private void LeftTime(float Values)
        {
            leftTime.value = Values;
        }

        public void Fulfill()
        {
            if (startTime != null)
            {
                StopCoroutine(startTime);
                startTime = null;
            }
            gameObject.SetActive(false);
        }

        public int GetScore()
        {
            return currentScore;
        }
    }
}

