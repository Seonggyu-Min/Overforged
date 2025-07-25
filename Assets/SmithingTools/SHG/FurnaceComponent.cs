using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using Void = EditorAttributes.Void;

public class FurnaceComponent : MonoBehaviour
{
    [SerializeField] [Required()]
    SmithingToolData furnaceData;
    Furnace furnace;
    [SerializeField] [VerticalGroup(10f, true, nameof(uiCanvas), nameof(itemImage), nameof(itemNameLabel), nameof(itemProgressLabel))]
    Void uiGroup;
    [SerializeField] [HideProperty]
    Canvas uiCanvas;
    [SerializeField] [HideProperty]
    Image itemImage;
    [SerializeField] [HideProperty]
    TMP_Text itemNameLabel;
    [SerializeField] [HideProperty]
    TMP_Text itemProgressLabel;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color interactColor;
    MeshRenderer meshRenderer;

    void BeforeInteract(SmithingTool tool, PlayerInteractArgs args)
    {
        Debug.Log($"Before Interact");
        Debug.Log(args);
    }

    void AfterInteract(SmithingTool tool, ToolInteractArgs args)
    {
        Debug.Log($"After Interact");
        Debug.Log(args);
    }

    void Awake()
    {
        this.furnace = new Furnace(this.furnaceData);
        this.furnace.BeforeInteract += this.BeforeInteract;
        this.furnace.AfterInteract += this.AfterInteract;
    }

  // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.furnace.OnUpdate(Time.deltaTime);
    }
}
