using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BoxButton : MonoBehaviour
{

    [SerializeField] Image image;

    [SerializeField] MaterialData matData;


    [SerializeField] public MaterialItemData data;
    [SerializeField] public OreType ore;
    [SerializeField] public WoodType wood;

    [SerializeField] TMP_Text text;

    void Start()
    {
        image.sprite = data.Image;
        if (ore != OreType.None) image.color = matData.oreColor[ore];
        if (wood != WoodType.None) image.color = matData.woodColor[wood];
        text.text = $"{matData.oreName[ore]}{matData.woodName[wood]}{data.name}";
    }
}
