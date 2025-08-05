using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine;
using Zenject;
using UnityEngine.SocialPlatforms;
using MIN;
using SHG;

namespace SCR
{
    public class OrderUI : MonoBehaviourPun
    {
        [SerializeField] InGameUIManager inGameUIManager;
        [SerializeField] List<RecipeUI> recipeUIs;
        [SerializeField] float repeatCycle;
        [SerializeField] private ItemDataList itemDataList;
        [SerializeField] private MaterialData materialData;
        [SerializeField] private ProductSprites productSprites; // 딕셔너리, 완성품 이미지들
        [Inject] IScoreManager _scoreManager;

        private int targetIndex = -1;

        public void StartOrder()
        {
            StartCoroutine(OrderCoroutine());
        }

        private IEnumerator OrderCoroutine()
        {
            while (!inGameUIManager.IsEnd)
            {
                float currentTime = repeatCycle;
                while (currentTime > 0)
                {
                    yield return new WaitForFixedUpdate();
                    currentTime -= Time.deltaTime;
                    if (CheckEmptyOrder())
                    {
                        AddRecipe();
                        currentTime = repeatCycle;
                    }
                }
                if (!CheckFullOrder())
                    AddRecipe();
            }

        }

        public void AddRecipe() // 특정 시간마다 호출 또는 다 껴져있을 때 호출
        {
            // if (!PhotonNetwork.IsMasterClient) return;

            //int index = Random.Range(0, allRecipes.Count);
            int prod = UnityEngine.Random.Range(0, itemDataList.craftList.Count);
            int ore = UnityEngine.Random.Range(1, materialData.ores.Count);
            int wood = UnityEngine.Random.Range(1, materialData.woods.Count);
            //photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, index, uiId);
            photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, prod, ore, wood);
        }

        [PunRPC]
        void RPC_AddRecipe(int prodIndex, int oreIndex, int woodIndex) //원래는 (int index, int uiId)
        {
            CraftData prod = itemDataList.craftList[prodIndex];
            WoodType wood = materialData.woods[woodIndex];
            OreType ore = OreType.None;
            if (prod.ProductItemData.productType != ProductType.Bow)
            {
                ore = materialData.ores[oreIndex];
            }
            //RecipeData recipe = allRecipes[index];
            RecipeUI ui = GetFirstUI();

            BotContext.Instance.AddRecipe(prod.ProductItemData, wood, ore); // AI에게 현재 레시피 정보 추가.

            ui.Setup(prod, wood, ore);
        }

        // 꺼져 있는 UI를 가져와서 첫번째에 둠
        private RecipeUI GetFirstUI()
        {
            foreach (RecipeUI recipeUI in recipeUIs)
            {
                if (!recipeUI.gameObject.activeSelf)
                {
                    recipeUI.gameObject.transform.SetAsFirstSibling();
                    return recipeUI;
                }
            }
            return null;
        }

        private bool CheckFullOrder()
        {
            foreach (RecipeUI recipeUI in recipeUIs)
            {
                if (!recipeUI.gameObject.activeSelf)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckEmptyOrder()
        {
            foreach (RecipeUI recipeUI in recipeUIs)
            {
                if (recipeUI.gameObject.activeSelf)
                {
                    return false;
                }
            }
            return true;
        }


        public bool Check(ProductItemData data, OreType ore, WoodType wood) // 출고시 호출
        {
            // 중복된 레시피중 하나만 제거하기 위함.
            // targetUI = curUIs.FirstOrDefault(ui =>
            //     ui.curProduct == data && ui.curOre == ore && ui.curWood == wood
            // );
            int index = -1;
            for (int i = 0; i < recipeUIs.Count; i++)
            {
                if (recipeUIs[i].gameObject.activeSelf)
                {
                    if (recipeUIs[i].CheckRecipe(data, ore, wood))
                    {
                        if (index < recipeUIs[i].gameObject.transform.GetSiblingIndex())
                        {
                            index = recipeUIs[i].gameObject.transform.GetSiblingIndex();
                            targetIndex = i;
                        }

                    }
                }
            }

            if (targetIndex != -1)
            {
                return true;
            }
            return false;
        }

        public void FulfillRecipe() // 출고시 호출
        {
            if (targetIndex != -1)
            {
                photonView.RPC(nameof(RPC_RemoveRecipe), RpcTarget.All, targetIndex);
                _scoreManager.AddScore(PhotonNetwork.LocalPlayer, recipeUIs[targetIndex].GetScore());
                targetIndex = -1;
            }
        }

        [PunRPC]
        void RPC_RemoveRecipe(int index)
        {
            recipeUIs[index].Fulfill();
        }
    }
}

