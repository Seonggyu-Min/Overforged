using System;
using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;

public class ProductTransfer : MonoBehaviour, IInteractableTool
{
    [SerializeField] ItemDataList itemdata;

    [SerializeField] MaterialData materialData;

    private ProductType prod;
    private OreType ore;
    private WoodType wood;

    private ProductItem item;

    [SerializeField] GameObject productItem;

    [SerializeField] Canvas UI;

    private List<ProductType> datas;
    private List<OreType> ores;
    private List<WoodType> woods;


    void Awake()
    {
        UI.enabled = false;
        for (int i = 0; i < int.MaxValue; i++)
        {
            if (Enum.IsDefined(typeof(ProductType), i))
            {
                datas[i] = (ProductType)i;
            }
            else break;
        }
        for (int i = 0; i < int.MaxValue; i++)
        {
            if (Enum.IsDefined(typeof(OreType), i))
            {
                ores[i] = (OreType)i;
            }
            else break;
        }
        for (int i = 0; i < int.MaxValue; i++)
        {
            if (Enum.IsDefined(typeof(WoodType), i))
            {
                woods[i] = (WoodType)i;
            }
            else break;
        }

    }


    public bool CanTransferItem(ToolTransferArgs args)
    {
        return true;
    }

    public ToolTransferResult Transfer(ToolTransferArgs args)
    {
        UI.enabled = false;
        return (new ToolTransferResult
        {
            ReceivedItem = null,
            IsDone = true
        });
    }

    public bool CanWork()
    {
        return true;
    }

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

    public void GenRequest()
    {
        int d = UnityEngine.Random.Range(0, datas.Count);
        int o = UnityEngine.Random.Range(1, ores.Count);
        int w = UnityEngine.Random.Range(1, woods.Count);

        prod = datas[d];
        wood = woods[w];
        if (prod != ProductType.Bow)
        {
            ore = ores[o];
        }
    }

    public void GenItem()
    {
        GameObject go = Instantiate(productItem);
        ProductItem HoldingItem = go.GetComponent<ProductItem>();

        if (prod == ProductType.Bow)
        {
            HoldingItem.Data = itemdata.Dict["Bow"];
        }
        else if (prod == ProductType.Sword)
        {
            HoldingItem.Data = itemdata.Dict["Sword"];
        }
        else if (prod == ProductType.Axe)
        {
            HoldingItem.Data = itemdata.Dict["Axe"];
        }
        else if (prod == ProductType.Hammer)
        {
            HoldingItem.Data = itemdata.Dict["Hammer"];
        }
        if (ore != OreType.None) HoldingItem.Ore = ore;
        if (wood != WoodType.None) HoldingItem.Wood = wood;

        TestPlayerControl player = GameObject.Find("Player").GetComponent<TestPlayerControl>();
        Transform hand = player.hand;
        player.current = HoldingItem;
        HoldingItem.Go(hand);
        UI.enabled = false;
    }

}
