using System.Collections;
using System.Collections.Generic;
using System.Data;
using SCR;
using SHG;
using TMPro;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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
    [SerializeField] BoxComponent oreBox;
    [SerializeField] BoxComponent woodBox;

    [SerializeField] TutorialTong tong;

    [SerializeField] TutorialRecipeManager trm;


    [SerializeField] GameObject targetingArrow;

    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text UIText;

    private MaterialItem material;

    private Dictionary<string, GameObject> interactables;
    private Dictionary<string, string> messages;
    [TextArea(3, 7)][SerializeField] string string_oreBox;
    [TextArea(3, 7)][SerializeField] string string_furnace;
    [TextArea(3, 7)][SerializeField] string string_anvil;
    [TextArea(3, 7)][SerializeField] string string_quenching;
    [TextArea(3, 7)][SerializeField] string string_woodBox;
    [TextArea(3, 7)][SerializeField] string string_table;
    [TextArea(3, 7)][SerializeField] string string_convey;

    [TextArea(3, 7)][SerializeField] string string_end;

    [TextArea(3, 7)][SerializeField] string string_escapeUI;

    [TextArea(3, 7)][SerializeField] string string_endUI;


    [Header("Plus Messages")]

    [TextArea(3, 7)][SerializeField] string string_onPutOre;

    [TextArea(3, 7)][SerializeField] string string_onWorkFurnace;

    [TextArea(3, 7)][SerializeField] string string_Tong;



    private Player player;
    private Transform targetTrs;
    private Transform playerTrs => player.transform;



    void Awake()
    {

        instance = this;
        IsTutorial = true;
        interactables = new();
        messages = new();
        //GameReadyAndStopManager.Instance.Skip = true;
        interactables.Add(nameof(anvil), anvil.gameObject);
        interactables.Add(nameof(table), table.gameObject);
        interactables.Add(nameof(quenching), quenching.gameObject);
        interactables.Add(nameof(furnace), furnace.gameObject);
        interactables.Add(nameof(convey), convey.gameObject);
        interactables.Add(nameof(oreBox), oreBox.gameObject);
        interactables.Add(nameof(woodBox), woodBox.gameObject);

        messages.Add(nameof(anvil), string_anvil);
        messages.Add(nameof(table), string_table);
        messages.Add(nameof(quenching), string_quenching);
        messages.Add(nameof(furnace), string_furnace);
        messages.Add(nameof(convey), string_convey);
        messages.Add(nameof(oreBox), string_oreBox);
        messages.Add(nameof(woodBox), string_woodBox);


    }
    void OnEnable()
    {
        oreBox.OnGetItem += OnOreGet;
        woodBox.OnGetItem += OnWoodGet;
        furnace.OnTransfered += OnFurnaceGet;
        anvil.OnTransfered += OnAnvilGet;
        table.OnTransfered += OnTableGet;
        convey.OnTransfered += OnConvey;

        anvil.OnWorked += OnFurnaceWork;

    }

    void Start()
    {
        SetNextTarget(nameof(oreBox));

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
    void OnFurnaceGet(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is MaterialItem item && item.Variation == MaterialVariation.Bar)
        {
            SetNextTarget(nameof(anvil));
            material = item;
            material.onCool += OnItemCool;
        }
        else if (result.ReceivedItem == null)
        {
            guideMessage.text = string_onPutOre;
            Debug.Log(123);
        }
        if (isFurnaceOn)
        {
            guideMessage.text = string_Tong;
            targetTrs = tong.gameObject.transform;
        }
    }
    void OnFurnaceWork(SmithingToolComponent comp, ToolWorkResult result)
    {
        isFurnaceOn = true;
        if (comp.HoldingItem != null)
        {
            guideMessage.text = string_Tong;
            targetTrs = tong.gameObject.transform;
        }



    }
    void OnAnvilGet(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is MaterialItem item && item.Variation == MaterialVariation.Blade)
        {
            SetNextTarget(nameof(quenching));
        }
    }

    void OnItemCool()
    {
        SetNextTarget(nameof(woodBox));
    }
    void OnTableGet(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        if (result.ReceivedItem is ProductItem item && item.Variation == ProductType.Sword)
        {
            SetNextTarget(nameof(convey));
        }
    }

    void OnConvey(SmithingToolComponent comp, ToolTransferArgs args, ToolTransferResult result)
    {
        trm.Fulfill();
        SetNextTarget("");
        guideMessage.text = string_end;
        IsTutorialEnd = true;
        UI.SetActive(true);
        UIText.text = string_endUI;
    }

    private void OnOreGet()
    {
        SetNextTarget(nameof(furnace));
    }
    private void OnWoodGet()
    {
        SetNextTarget(nameof(table));
    }

    private void SetNextTarget(string name)
    {
        foreach (KeyValuePair<string, GameObject> kv in interactables)
        {
            if (kv.Key == name)
            {
                kv.Value.layer = 7;
                targetTrs = kv.Value.transform;
                guideMessage.text = messages[name];
            }
            else
            {
                kv.Value.layer = 0;
            }
        }
    }
    void OnDisable()
    {
        oreBox.OnGetItem -= OnOreGet;
        woodBox.OnGetItem -= OnWoodGet;
        furnace.OnTransfered -= OnFurnaceGet;
        anvil.OnTransfered -= OnAnvilGet;
        table.OnTransfered -= OnTableGet;
        convey.OnTransfered -= OnConvey;
        material.onCool -= OnItemCool;

    }

}
