using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace JJY
{
    public class RecipeManager : MonoBehaviourPun
    {
        // TODO : photonNetwork를 이용해서 동일한 레시피 UI를 모든 플레이어에게 띄울 필요가 있음.
        // 레시피대로 만들고 출품고에 출품을하면 모든 플레이어에게 해당 레시피 UI가 사라져야함.
        [SerializeField] private List<RecipeData> allRecipes;
        [SerializeField] private Transform recipeUIParent; // 왼쪽 위에 정렬될 위치
        [SerializeField] private GameObject recipeUIPrefab;

        private List<RecipeUI> curUIs = new();

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
            int index = Random.Range(0, allRecipes.Count);
            photonView.RPC(nameof(RPC_AddRecipe), RpcTarget.AllBuffered, index);
        }

        [PunRPC]
        void RPC_AddRecipe(int index)
        {
            RecipeData recipe = allRecipes[index];
            GameObject go = Instantiate(recipeUIPrefab, recipeUIParent);
            RecipeUI ui = go.GetComponent<RecipeUI>();

            ui.Setup(recipe);
            curUIs.Add(ui);

            ReorderUI();
        }

        public void FulfillRecipe(RecipeData targetRecipe) // 출고시 호출
        {
            photonView.RPC(nameof(RPC_RemoveRecipe), RpcTarget.All, targetRecipe.name);
        }

        [PunRPC]
        void RPC_RemoveRecipe(string recipeName)
        {
            // 중복된 레시피중 하나만 제거하기 위함.
            var toRemove = curUIs.FirstOrDefault(ui => ui.curRecipe.recipeName == recipeName);
            
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
