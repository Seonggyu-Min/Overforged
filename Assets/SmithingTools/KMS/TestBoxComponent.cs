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


    void Awake()
    {

    }


    public bool CanTransferItem(ToolTransferArgs args)
    {
        //손에 든 것이 없을때만 가능하도록
        return (args.ItemToGive == null);

    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
        GenerateItem();
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
        return false;

    }

    public ToolWorkResult Work()
    {
        return new ToolWorkResult();
    }

    private void GenerateItem()
    {
        GameObject go = Instantiate(matItem);
        go.transform.position = transform.position;
        HoldingItem = go.GetComponent<MaterialItem>();
        HoldingItem.Data = itemdata.list[3];
        HoldingItem.Ore = OreType.Gold;
        Debug.Log(HoldingItem.Name);
    }
}
