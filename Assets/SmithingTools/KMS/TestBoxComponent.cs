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

    [SerializeField] Transform hand;


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

    private void GenerateItem(MaterialItemData data, OreType ore, WoodType wood)
    {
        GameObject go = Instantiate(matItem);
        HoldingItem = go.GetComponent<MaterialItem>();
        HoldingItem.Data = data;
        if (ore != OreType.None) HoldingItem.Ore = ore;
        if (wood != WoodType.None) HoldingItem.Wood = wood;

        TestPlayerControl player = GameObject.Find("Player").GetComponent<TestPlayerControl>();
        Transform hand = player.hand;
        player.current = HoldingItem;
        HoldingItem.Go(hand);
        HoldingItem = null;
        UI.enabled = false;
    }

    public void ButtonClick(BoxButton btn)
    {
        GenerateItem(btn.data, btn.ore, btn.wood);

    }
}
