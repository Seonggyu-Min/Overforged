using System.Collections;
using System.Collections.Generic;
//using System.Data;
using SCR;
using SHG;
using TMPro;
using Unity.AppUI.UI;
//using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.PackageManager.Requests;
//using UnityEditor.Splines;
using UnityEngine;
//using UnityEngine.Rendering;
using WebSocketSharp;

public class TutorialManager : MonoBehaviour
{

    private static TutorialManager instance;

    public static TutorialManager Instance { get { return instance; } }

    public bool IsTutorial { get; private set; }

    public bool IsTutorialEnd { get; private set; }

    [SerializeField] TMP_Text guideMessage;


    [SerializeField] AnvilComponent anvil;
    [SerializeField] TableComponent table;
    [SerializeField] QuenchingComponent quenching;
    [SerializeField] FurnaceComponent furnace;
    [SerializeField] NewProductConveyComponent convey;
    [SerializeField] DropOffTableComponent dropoff;
    [SerializeField] BoxComponent oreBox;
    [SerializeField] BoxComponent woodBox;

    [SerializeField] TutorialTong tong;

    [SerializeField] TutorialRecipeManager trm;


    [SerializeField] GameObject targetingArrow;

    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text UIText;

    private MaterialItem metalMaterial;
    private MaterialItem woodMaterial;

    private Dictionary<string, GameObject> interactables;
    [TextArea(3, 7)][SerializeField] string string_OreBox;
    [TextArea(3, 7)][SerializeField] string string_WoodBox;

    [TextArea(3, 7)][SerializeField] string string_FurnaceToPutOre;
    [TextArea(3, 7)][SerializeField] string string_FurnaceToGetBar;
    [TextArea(3, 7)][SerializeField] string string_FurnaceIsNotDone;
    [TextArea(3, 7)][SerializeField] string string_FurnaceOn;

    [TextArea(3, 7)][SerializeField] string string_AnvilToPutBar;
    [TextArea(3, 7)][SerializeField] string string_AnvilToMakeBlade;
    [TextArea(3, 7)][SerializeField] string string_AnvilToGetBlade;

    [TextArea(3, 7)][SerializeField] string string_QuenchingToCool;
    [TextArea(3, 7)][SerializeField] string string_QuenchingToGetBlade;
    [TextArea(3, 7)][SerializeField] string string_QuenchingIsNotDone;

    [TextArea(3, 7)][SerializeField] string string_DropoffToPutBlade;
    [TextArea(3, 7)][SerializeField] string string_DropoffToGetBlade;

    [TextArea(3, 7)][SerializeField] string string_TableToMakeHandle;
    [TextArea(3, 7)][SerializeField] string string_TableToMakeSword;

    [TextArea(3, 7)][SerializeField] string string_Convey;
    [TextArea(3, 7)][SerializeField] string string_End;

    [TextArea(3, 7)][SerializeField] string string_GetTongToGetBar;
    [TextArea(3, 7)][SerializeField] string string_GetTongToGetBlade;
    [TextArea(3, 7)][SerializeField] string string_LeaveTongToMakeBlade;
    [TextArea(3, 7)][SerializeField] string string_LeaveTongToGetBlade;

    [TextArea(3, 7)][SerializeField] string string_escapeUI;
    [TextArea(3, 7)][SerializeField] string string_endUI;

    private Player player;
    private Transform targetTrs;
    private Transform playerTrs => player.transform;

    private Coroutine stringCo;
    private WaitForSeconds wfs;

    void Awake()
    {
        instance = this;
        IsTutorial = true;
        interactables = new();
        wfs = new WaitForSeconds(0.04f);
        //GameReadyAndStopManager.Instance.Skip = true;
        interactables.Add(nameof(anvil), anvil.gameObject);
        interactables.Add(nameof(table), table.gameObject);
        interactables.Add(nameof(quenching), quenching.gameObject);
        interactables.Add(nameof(furnace), furnace.gameObject);
        interactables.Add(nameof(convey), convey.gameObject);
        interactables.Add(nameof(oreBox), oreBox.gameObject);
        interactables.Add(nameof(woodBox), woodBox.gameObject);
        interactables.Add(nameof(tong), tong.gameObject);
        interactables.Add(nameof(dropoff), dropoff.gameObject);
    }
    void OnEnable()
    {
        oreBox.OnGetItem += OnOreGet;
        woodBox.OnGetItem += OnWoodGet;
        furnace.OnTransfered += OnFurnaceTransfer;
        anvil.OnTransfered += OnAnvilTransfer;
        table.OnTransfered += OnTableTransfer;
        convey.OnTransfered += OnConvey;

        furnace.OnWorked += OnFurnaceWork;
        tong.OnGet += OnGetTong;
        tong.OnAbandon += OnLeaveTong;
        quenching.OnTransfered += OnQuenchingTransfer;
        dropoff.OnDropOffTransfered += OnDropoffTransfer;


    }

    void Start()
    {
        SetNextTarget(nameof(oreBox));
        SetGuideString(string_OreBox);

        UIText.text = string_escapeUI;
        UI.SetActive(false);
    }

    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            return;
        }
        if (targetTrs == null) return;

        Vector3 direct = targetTrs.position - playerTrs.position;
        Vector3 middlePos = direct * 0.5f + player.transform.position;
        float mag = Mathf.Min(1f, direct.magnitude * 0.2f);
        if (mag <= 0.4f) mag = 0;
        Vector3 scale = Vector3.one * mag;
        targetingArrow.transform.forward = direct.normalized;
        targetingArrow.transform.position = middlePos;
        targetingArrow.transform.localScale = scale;

        TryToggleUI();

    }

    private void TryToggleUI()
    {
        if (IsTutorialEnd) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UI.activeSelf)
            {
                UI.SetActive(false);

            }
            else
            {
                UI.SetActive(true);
            }

        }
    }
    private bool isFurnaceOn;
    private bool isOreIgnite;
    void OnFurnaceTransfer(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is MaterialItem item && item.Variation == MaterialVariation.Bar)
        {
            SetNextTarget(nameof(anvil));
            SetGuideString(string_AnvilToPutBar);
            return;
        }
        else if (result.ReceivedItem == null)
        {
            SetGuideString(string_FurnaceOn);
        }
        if (isFurnaceOn)
        {
            SetNextTarget(nameof(tong));
            SetGuideString(string_GetTongToGetBar);
            shouldGetTong = true;
            isOreIgnite = true;

        }
    }
    void OnFurnaceWork(SmithingToolComponent comp, ToolWorkResult result)
    {
        isFurnaceOn = true;
        if (comp.HoldingItem != null)
        {
            SetNextTarget(nameof(tong));
            SetGuideString(string_GetTongToGetBar);
            shouldGetTong = true;
            isOreIgnite = true;
        }
    }
    private bool shouldGetTong;
    private bool shouldLeaveTong;

    void OnGetTong()
    {
        if (!shouldGetTong) return;
        if (oreChangedToBar)
        {
            SetNextTarget(nameof(furnace));
            SetGuideString(string_FurnaceToGetBar);
            oreChangedToBar = false;
        }
        else if (isOreIgnite)
        {
            SetNextTarget("");
            SetGuideString(string_FurnaceIsNotDone);
            isOreIgnite = false;
        }
        else if (barChangedToBlade)
        {
            SetNextTarget(nameof(anvil));
            SetGuideString(string_AnvilToGetBlade);
            barChangedToBlade = false;

        }
        shouldGetTong = false;
    }
    void OnLeaveTong()
    {
        if (!shouldLeaveTong) return;
        if (isBarPut)
        {
            SetNextTarget(nameof(anvil));
            SetGuideString(string_AnvilToMakeBlade);
            isBarPut = false;
        }
        else if (isItemCool)
        {
            SetNextTarget(nameof(quenching));
            SetGuideString(string_QuenchingToGetBlade);
            isItemCool = false;
        }
        shouldLeaveTong = false;


    }
    private bool isBarPut;
    void OnAnvilTransfer(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is MaterialItem item && item.Variation == MaterialVariation.Blade)
        {
            SetNextTarget(nameof(quenching));
            SetGuideString(string_QuenchingToCool);
        }
        else if (result.ReceivedItem == null && comp.HoldingItem is MaterialItem i && i.Variation == MaterialVariation.Bar)
        {
            SetNextTarget("");
            SetGuideString(string_LeaveTongToMakeBlade);
            shouldLeaveTong = true;
            isBarPut = true;
        }
    }
    private bool oreChangedToBar;
    private bool barChangedToBlade;

    void OnMetalItemChange(MaterialItemData data)
    {
        if (data.materialVariation == MaterialVariation.Blade)
        {
            SetNextTarget(nameof(tong));
            SetGuideString(string_GetTongToGetBlade);
            shouldGetTong = true;
            barChangedToBlade = true;
        }
        else if (data.materialVariation == MaterialVariation.Bar)
        {
            oreChangedToBar = true;
            isOreIgnite = false;
            if (!shouldGetTong)
            {
                SetNextTarget(nameof(furnace));
                SetGuideString(string_FurnaceToGetBar);
                oreChangedToBar = false;
            }

        }

    }
    void OnWoodItemChange(MaterialItemData data)
    {
        if (data.materialVariation == MaterialVariation.Handle)
        {
            SetNextTarget(nameof(dropoff));
            SetGuideString(string_DropoffToGetBlade);

        }
    }

    void OnQuenchingTransfer(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is MaterialItem item && item.Variation == MaterialVariation.Blade)
        {
            SetNextTarget(nameof(dropoff));
            SetGuideString(string_DropoffToPutBlade);
        }
        else if (result.ReceivedItem == null)
        {
            SetNextTarget("");
            SetGuideString(string_QuenchingIsNotDone);
        }

    }
    private bool isItemCool;

    void OnItemCool()
    {
        if (tong.gameObject.activeSelf)
        {
            SetNextTarget(nameof(quenching));
            SetGuideString(string_QuenchingToGetBlade);
        }
        else
        {
            SetNextTarget(nameof(tong));
            SetGuideString(string_LeaveTongToGetBlade);
            shouldLeaveTong = true;
            isItemCool = true;
        }
    }

    void OnDropoffTransfer(ToolTransferResult result)
    {
        if (result.ReceivedItem is MaterialItem item && item.Variation == MaterialVariation.Blade)
        {
            SetNextTarget(nameof(table));
            SetGuideString(string_TableToMakeSword);
        }
        else if (result.ReceivedItem == null)
        {
            SetNextTarget(nameof(woodBox));
            SetGuideString(string_WoodBox);
        }

    }
    void OnTableTransfer(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is ProductItem item && item.Variation == ProductType.Sword)
        {
            SetNextTarget(nameof(convey));
            SetGuideString(string_Convey);
        }
    }

    void OnConvey(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        trm.Fulfill();
        SetNextTarget("");
        guideMessage.text = string_End;
        IsTutorialEnd = true;
        UI.SetActive(true);
        UIText.text = string_endUI;
    }

    private void OnOreGet(GameObject go)
    {
        SetNextTarget(nameof(furnace));
        SetGuideString(string_FurnaceToPutOre);
        metalMaterial = go.GetComponent<MaterialItem>();
        metalMaterial.onCool += OnItemCool;
        metalMaterial.onChangeNext += OnMetalItemChange;
    }
    private void OnWoodGet(GameObject go)
    {
        SetNextTarget(nameof(table));
        SetGuideString(string_TableToMakeHandle);
        woodMaterial = go.GetComponent<MaterialItem>();
        woodMaterial.onChangeNext += OnWoodItemChange;
    }

    private void SetNextTarget(string name)
    {
        if (name.IsNullOrEmpty()) targetTrs = null;
        foreach (KeyValuePair<string, GameObject> kv in interactables)
        {
            if (kv.Key == name)
            {
                if (kv.Key == nameof(tong))
                {
                    kv.Value.layer = 6;
                }
                else
                {
                    kv.Value.layer = 7;
                }
                targetTrs = kv.Value.transform;
            }
            else
            {
                if (kv.Key != nameof(tong))
                {
                    kv.Value.layer = 0;
                }
            }
        }
    }

    private void SetGuideString(string str)
    {
        if (stringCo != null)
        {
            StopCoroutine(stringCo);
            stringCo = null;
            stringCo = StartCoroutine(StringRoutine(str));
        }
        else
        {
            stringCo = StartCoroutine(StringRoutine(str));
        }
    }


    private IEnumerator StringRoutine(string str)
    {
        guideMessage.text = "";
        foreach (char c in str)
        {
            guideMessage.text = $"{guideMessage.text}{c}";
            yield return wfs;
        }

    }
    void OnDisable()
    {
        oreBox.OnGetItem -= OnOreGet;
        woodBox.OnGetItem -= OnWoodGet;
        furnace.OnTransfered -= OnFurnaceTransfer;
        anvil.OnTransfered -= OnAnvilTransfer;
        table.OnTransfered -= OnTableTransfer;
        convey.OnTransfered -= OnConvey;

        furnace.OnWorked -= OnFurnaceWork;
        tong.OnGet -= OnGetTong;
        tong.OnAbandon -= OnLeaveTong;
        quenching.OnTransfered -= OnQuenchingTransfer;
        dropoff.OnDropOffTransfered -= OnDropoffTransfer;
        if (metalMaterial != null)
        {
            metalMaterial.onCool -= OnItemCool;
            metalMaterial.onChangeNext -= OnMetalItemChange;

        }
        if (woodMaterial != null)
        {
            woodMaterial.onChangeNext -= OnWoodItemChange;

        }
    }
}
