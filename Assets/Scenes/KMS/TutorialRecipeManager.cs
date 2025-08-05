using System.Collections;
using System.Collections.Generic;
using SCR;
using UnityEngine;

public class TutorialRecipeManager : MonoBehaviour
{


    [SerializeField] RecipeUI rui;

    [SerializeField] ItemDataList itemDataList;
    //[SerializeField] MaterialData materialData;

    [SerializeField] private ProductSprites productSprites; // 딕셔너리, 완성품 이미지들
    void Start()
    {
        productSprites.Init(); // 딕셔너리 초기화작업.
        Init();
    }
    void Update()
    {
        if (!rui.gameObject.activeSelf)
        {
            rui.gameObject.SetActive(true);
        }
    }

    public void Fulfill()
    {
        rui.Fulfill();
    }

    private void Init()
    {
        rui.gameObject.SetActive(true);
        CraftData prod = itemDataList.craftList[0];
        WoodType wood = WoodType.Oak;
        OreType ore = OreType.Steel;
        rui.Setup(prod, wood, ore);

    }
}
