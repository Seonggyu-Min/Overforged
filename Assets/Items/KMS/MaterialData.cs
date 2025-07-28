using System.Collections;
using System.Collections.Generic;
using MIN;
using UnityEngine;

[CreateAssetMenu(menuName = "MaterialData")]
public class MaterialData : ScriptableObject
{

    public Material Gold;
    public Material Copper;
    public Material Steel;
    public Material Wood;

    public Material Stone;
    public Material None;

    public Material Birch;

    public Material HotMetal;

    public List<Color> colors;

    public Dictionary<OreType, Material> OreDict;
    public Dictionary<WoodType, Material> WoodDict;

    public Dictionary<OreType, string> oreName;
    public Dictionary<WoodType, string> woodName;

    public Dictionary<OreType, Color> oreColor;
    public Dictionary<WoodType, Color> woodColor;

    void OnEnable()
    {
        OreDict = new();
        OreDict.Add(OreType.None, None);
        OreDict.Add(OreType.Steel, Steel);
        OreDict.Add(OreType.Copper, Copper);
        OreDict.Add(OreType.Gold, Gold);
        WoodDict = new();
        WoodDict.Add(WoodType.None, None);
        WoodDict.Add(WoodType.Oak, Wood);
        WoodDict.Add(WoodType.Birch, Birch);

        oreName = new();
        woodName = new();
        oreName.Add(OreType.None, "");
        oreName.Add(OreType.Copper, "Copper ");
        oreName.Add(OreType.Steel, "Steel ");
        oreName.Add(OreType.Gold, "Gold ");
        woodName.Add(WoodType.None, "");
        woodName.Add(WoodType.Oak, "Oak ");
        woodName.Add(WoodType.Birch, "Birch ");

        oreColor = new();
        woodColor = new();

        oreColor.Add(OreType.None, colors[0]);
        oreColor.Add(OreType.Copper, colors[1]);
        oreColor.Add(OreType.Steel, colors[2]);
        oreColor.Add(OreType.Gold, colors[3]);
        woodColor.Add(WoodType.None, colors[4]);
        woodColor.Add(WoodType.Oak, colors[5]);
        woodColor.Add(WoodType.Birch, colors[6]);

        


    }


}
