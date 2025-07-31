using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using System;
using Zenject;

namespace JJY
{
    public class RecipeManager : MonoBehaviourPun
    {
        // [SerializeField] private List<RecipeData> allRecipes;
        [SerializeField] private Transform recipeUIParent; // 왼쪽 위에 정렬될 위치
        [SerializeField] private GameObject recipeUIPrefab;
        private int recipeUICounter = 0; // 유니크 ID용 카운터

        private List<RecipeUI> curUIs = new();

        [SerializeField] private ItemDataList itemDataList;
        [SerializeField] private MaterialData materialData;
        [SerializeField] private ProductSprites productSprites; // 딕셔너리, 완성품 이미지들

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
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
        void Start()
        {
            productSprites.Init(); // 딕셔너리 초기화작업.

            // if (!PhotonNetwork.IsMasterClient) return;

            for (int i = 0; i < 5; i++)
            {
                SpawnRandomRecipe();
            }
        }

        public void SpawnRandomRecipe() // 게임 시작시 호출
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            //int index = Random.Range(0, allRecipes.Count);
            int p = UnityEngine.Random.Range(0, itemDataList.craftList.Count);
            int o = UnityEngine.Random.Range(1, materialData.ores.Count);
            int w = UnityEngine.Random.Range(1, materialData.woods.Count);
            int uiId = recipeUICounter++;
            //photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, index, uiId);
            photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, p, o, w, uiId);
            SpawnItemTest(p, o, w);
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
        int itemid;

        void SpawnItemTest(int p, int o, int w)
        {
            GameObject i = PhotonNetwork.Instantiate("ProductItem", new Vector3(0, 2, 0), Quaternion.identity);
            itemid = i.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(SetItemTest), RpcTarget.AllBuffered, itemid, p, o, w);
        }
        [PunRPC]
        void SetItemTest(int id, int p, int o, int w)
        {
            ProductItem pi = PhotonView.Find(id).GetComponent<ProductItem>();
            ProductItemData prod = itemDataList.craftList[p].ProductItemData;
            WoodType wood = materialData.woods[w];
            OreType ore = OreType.None;
            if (prod.productType != ProductType.Bow)
            {
                ore = materialData.ores[o];
            }
            pi.Data = prod;
            pi.Ore = ore;
            pi.Wood = wood;
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
        RecipeUI targetUI = null;
        public bool Check(ProductItemData data, OreType ore, WoodType wood) // 출고시 호출
        {
            // 중복된 레시피중 하나만 제거하기 위함.
            targetUI = curUIs.FirstOrDefault(ui =>
                ui.curProduct == data && ui.curOre == ore && ui.curWood == wood
            );
            if (targetUI != null)
            {
                //photonView.RPC(nameof(RPC_RemoveRecipe), RpcTarget.All, targetUI.uniqueID);
                return true;
            }
            return false;
        }

        public void FulfillRecipe() // 출고시 호출
        {
            if (targetUI != null)
            {
                photonView.RPC(nameof(RPC_RemoveRecipe), RpcTarget.All, targetUI.uniqueID);
                targetUI = null;

                SpawnRandomRecipe(); // TEST : 출고시 새로운 레시피 재생성
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
