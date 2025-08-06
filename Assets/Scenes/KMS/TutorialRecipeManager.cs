using System.Collections;
using System.Collections.Generic;
using SCR;
using UnityEngine;

public class TutorialRecipeManager : MonoBehaviour
{


    [SerializeField] RecipeUI rui;

    [SerializeField] ItemDataList itemDataList;
    //[SerializeField] MaterialData materialData;

    public bool IsFulFilled;

    [SerializeField] private ProductSprites productSprites; // 딕셔너리, 완성품 이미지들
    void Start()
    {
        productSprites.Init(); // 딕셔너리 초기화작업.
        Init();
    }
    void Update()
    {
        if (IsFulFilled)
        {
            rui.gameObject.SetActive(false);
        }
        else if (!rui.gameObject.activeSelf && !IsFulFilled)
        {
            rui.gameObject.SetActive(true);
        }
    }

    public void Fulfill()
    {
        IsFulFilled = true;
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
