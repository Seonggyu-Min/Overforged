using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using System;

namespace JJY
{
    public class RecipeManager : MonoBehaviourPun
    {
        // TODO : photonNetwork를 이용해서 동일한 레시피 UI를 모든 플레이어에게 띄울 필요가 있음.
        // 레시피대로 만들고 출품고에 출품을하면 모든 플레이어에게 해당 레시피 UI가 사라져야함.
        [SerializeField] private List<RecipeData> allRecipes;
        [SerializeField] private Transform recipeUIParent; // 왼쪽 위에 정렬될 위치
        [SerializeField] private GameObject recipeUIPrefab;
        private int recipeUICounter = 0; // 유니크 ID용 카운터

        private List<RecipeUI> curUIs = new();

        [SerializeField] private ItemDataList itemDataList;
        [SerializeField] private MaterialData materialData;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnRandomRecipe();
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (curUIs.Count > 0)
                {
                    FulfillRecipe(curUIs[0].curRecipe);
                }
            }
        }

        public void SpawnRandomRecipe() // 게임 시작시 호출
        {
            //int index = Random.Range(0, allRecipes.Count);
            int p = UnityEngine.Random.Range(0, itemDataList.craftList.Count);
            int o = UnityEngine.Random.Range(1, materialData.ores.Count);
            int w = UnityEngine.Random.Range(1, materialData.woods.Count);
            int uiId = recipeUICounter++;
            //photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, index, uiId);
            photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, p, o, w, uiId);
        }

        [PunRPC]
        void RPC_AddRecipe(int p, int o, int w, int uiId) //원래는 (int index, int uiId)
        {
            CraftData prod = itemDataList.craftList[p];
            WoodType wood = materialData.woods[w];
            OreType ore = OreType.None;
            if (prod.ProductItemData.productType != ProductType.Bow)
            {
                ore = materialData.ores[o];
            }
            //RecipeData recipe = allRecipes[index];
            GameObject go = Instantiate(recipeUIPrefab, recipeUIParent);
            RecipeUI ui = go.GetComponent<RecipeUI>();

            //ui.Setup(recipe, uiId);
            ui.Setup(prod, wood, ore, uiId);
            curUIs.Add(ui);

            ReorderUI();
        }

        public void FulfillRecipe(RecipeData targetRecipe) // 출고시 호출
        {
            // 중복된 레시피중 하나만 제거하기 위함.
            RecipeUI targetUI = curUIs.FirstOrDefault(ui => ui.curRecipe == targetRecipe);
            if (targetUI != null)
            {
                photonView.RPC(nameof(RPC_RemoveRecipe), RpcTarget.All, targetUI.uniqueID);
            }
        }

        public void FulfillRecipe(ProductItemData data, OreType ore, WoodType wood) // 출고시 호출
        {
            // 중복된 레시피중 하나만 제거하기 위함.
            RecipeUI targetUI = curUIs.FirstOrDefault(ui =>
                ui.curProduct == data && ui.curOre == ore && ui.curWood == wood
            );
            if (targetUI != null)
            {
                photonView.RPC(nameof(RPC_RemoveRecipe), RpcTarget.All, targetUI.uniqueID);
            }
        }

        [PunRPC]
        void RPC_RemoveRecipe(int uiId)
        {
            // 중복된 레시피중 하나만 제거하기 위함.
            var toRemove = curUIs.FirstOrDefault(ui => ui.uniqueID == uiId);

            if (toRemove != null)
            {
                curUIs.Remove(toRemove);
                Destroy(toRemove.gameObject);
                ReorderUI();
            }
        }

        void ReorderUI()
        {
            for (int i = 0; i < curUIs.Count; i++)
            {
                curUIs[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
