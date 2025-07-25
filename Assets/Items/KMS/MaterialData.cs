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

        public Dictionary<OreType, Material> OreDict;
        public Dictionary<WoodType, Material> WoodDict;

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
        }


    }   
