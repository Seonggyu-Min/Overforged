using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using Void = EditorAttributes.Void;
using Zenject;

public class TestBoxComponent : MonoBehaviour, IInteractableTool
{

    [SerializeField] ItemDataList itemdata;

    private MaterialItem HoldingItem;

    [SerializeField] GameObject matItem;

    [SerializeField] Canvas UI;


    void Awake()
    {
        UI.enabled = false;
    }


    public bool CanTransferItem(ToolTransferArgs args)
    {
        return false;
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
        UI.enabled = false;
        var item = HoldingItem;
        this.HoldingItem = null;
        return (new ToolTransferResult
        {
            ReceivedItem = item,
            IsDone = true
        });
    }

    public bool CanWork()
    {
        return true;
    }


    //상호작용 시 UI가 켜져있으면 끄고, 꺼져있으면 키고 못움직이도록 해보았습니다.

    public ToolWorkResult Work()
    {
        if (UI.enabled)
        {
            UI.enabled = false;
            return (new ToolWorkResult
            {
                Trigger = null,
                DurationToStay = 0.1f
            });

        }
        else
        {
            UI.enabled = true;
            return (new ToolWorkResult
            {
                Trigger = null,
                DurationToStay = float.MaxValue
            });
        }
    }

    private void GenerateItem(int id, OreType ore, WoodType wood)
    {
        GameObject go = Instantiate(matItem);
        HoldingItem = go.GetComponent<MaterialItem>();
        HoldingItem.Data = itemdata.list[id];
        if (ore != OreType.None) HoldingItem.Ore = ore;
        if (wood != WoodType.None) HoldingItem.Wood = wood;
        Transfer(new ToolTransferArgs());
    }

    public void GenIronOre()
    {
        GenerateItem(3, OreType.Steel, WoodType.None);
    }
    public void GenCopperOre()
    {
        GenerateItem(3, OreType.Copper, WoodType.None);
    }
    public void GenGoldOre()
    {
        GenerateItem(3, OreType.Gold, WoodType.None);
    }
    public void GenOakWood()
    {
        GenerateItem(0, OreType.None, WoodType.Oak);
    }
    public void GenBirchWood()
    {
        GenerateItem(0, OreType.None, WoodType.Birch);
    }
    public void GenString()
    {
        GenerateItem(2, OreType.None, WoodType.None);
    }
}
