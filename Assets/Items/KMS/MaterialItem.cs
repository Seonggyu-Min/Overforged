using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIN;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System;

public class MaterialItem : Item
{

    private OreType ore;
    public OreType Ore
    {
        get
        {
            return ore;
        }
        set
        {
            if (matData.materialType == MaterialType.Metallic || matData.materialType == MaterialType.Mineral)
            {
                ore = value;   
            }
            SetMaterial();
        }
    }
    private WoodType wood;
    public WoodType Wood
    {
        get
        {
            return wood;
        }
        set
        {
            if (matData.materialType == MaterialType.Wooden)
            {
                wood = value;
            }
            SetMaterial();
        }
    }

    private MeshFilter mesh;
    private MeshRenderer render;

    private Material CurrentOreMat => matCatalog.OreDict[ore];
    private Material CurrentWoodMat => matCatalog.WoodDict[wood];

    public Color CurrentOreSpriteColor => matCatalog.oreColor[ore];
    public Color CurrentWoodSpriteColor => matCatalog.woodColor[wood];

    private MaterialItemData matData;

    public MaterialVariation Variation => matData.materialVariation;

    public override string Name
    {
        get
        {
            return $"{matCatalog.woodName[wood]}{matCatalog.oreName[ore]}{data.name}";
        }
    }

    protected override void Awake()
    {
        base.Awake();
        mesh = model.GetComponent<MeshFilter>();
        render = model.GetComponent<MeshRenderer>();
        ore = OreType.None;
        wood = WoodType.None;
    }

    protected override void InitItemData(ItemData itemdata)
    {
        matData = itemdata as MaterialItemData;
        mesh.sharedMesh = data.ItemPrefab.GetComponent<MeshFilter>().sharedMesh;
        render.sharedMaterials = data.ItemPrefab.GetComponent<MeshRenderer>().sharedMaterials;
        SetMaterial();
        if (isHot) render.sharedMaterial = matCatalog.HotMetal;

    }
    [PunRPC]
    public void Heat()
    {
        isHot = true;
        if (matData.materialType == MaterialType.Metallic)
        {
            render.sharedMaterial = matCatalog.HotMetal;

        }
        else if (matData.materialType == MaterialType.Mineral)
        {
            render.sharedMaterials = new Material[] { matCatalog.HotMetal, matCatalog.HotMetal };
        }
        highlighter = null;
        highlighter = new SHG.GameObjectHighlighter(render.materials);
        this.render.materials = this.highlighter.HighlightedMaterials;
    }

    public Action onCool;
    public Action<MaterialItemData> onChangeNext;
    [PunRPC]
    public void Cool()
    {
        isHot = false;
        SetMaterial();
        onCool?.Invoke();
    }
    [PunRPC]
    public void ChangeToNext()
    {
        if (data == null) return;
        Data = matData.nextMaterial;
        onChangeNext?.Invoke(matData);
    }

    private void SetMaterial()
    {
        if (matData.materialType == MaterialType.Metallic)
        {
            render.sharedMaterial = CurrentOreMat;

        }
        else if (matData.materialType == MaterialType.Wooden)
        {
            render.sharedMaterial = CurrentWoodMat;

        }
        else if (matData.materialType == MaterialType.Mineral)
        {
            render.sharedMaterials = new Material[] { matCatalog.Stone, CurrentOreMat };
        }
        else
        {
            render.sharedMaterial = matCatalog.OreDict[OreType.None];
        }
        highlighter = new SHG.GameObjectHighlighter(render.materials);
        this.render.materials = this.highlighter.HighlightedMaterials;

    }
    public static OreType GetOreType(List<MaterialItem> list)
    {
        foreach (var mat in list)
        {
            if (mat.matData.materialType == MaterialType.Metallic)
            {
                return mat.Ore;
            }
        }
        return OreType.None;
    }
    public static WoodType GetWoodType(List<MaterialItem> list)
    {
        foreach (var mat in list)
        {
            if (mat.matData.materialType == MaterialType.Wooden)
            {
                return mat.Wood;
            }
        }
        return WoodType.None;
    }
}
