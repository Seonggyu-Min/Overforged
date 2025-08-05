using System.Collections;
using System.Collections.Generic;
using System.Data;
using SCR;
using SHG;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    private static TutorialManager instance;

    public static TutorialManager Instance { get { return instance; } }

    public bool IsTutorial { get; private set; }

    [SerializeField] TMP_Text guideMessage;


    [SerializeField] AnvilComponent anvil;
    [SerializeField] TableComponent table;
    [SerializeField] QuenchingComponent quenching;
    [SerializeField] FurnaceComponent furnace;
    [SerializeField] NewProductConveyComponent convey;
    [SerializeField] BoxComponent oreBox;
    [SerializeField] BoxComponent woodBox;


    [SerializeField] GameObject targetingArrow;

    private Dictionary<string, GameObject> interactables;

    void Awake()
    {

        instance = this;
        IsTutorial = true;
        interactables = new();
        GameReadyAndStopManager.Instance.Skip = true;
        interactables.Add(nameof(anvil), anvil.gameObject);
        interactables.Add(nameof(table), table.gameObject);
        interactables.Add(nameof(quenching), quenching.gameObject);
        interactables.Add(nameof(furnace), furnace.gameObject);
        interactables.Add(nameof(convey), convey.gameObject);
        interactables.Add(nameof(oreBox), oreBox.gameObject);
        interactables.Add(nameof(woodBox), woodBox.gameObject);

    }
    void OnEnable()
    {
        oreBox.OnGetItem += OnOreGet;
        woodBox.OnGetItem += OnWoodGet;

    }

    void Start()
    {
        SetNextTarget(nameof(oreBox));
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
                targetingArrow.transform.position = kv.Value.transform.position + new Vector3(0, 3, 0);
            }
            else
            {
                kv.Value.layer = 0;
            }
        }
    }

}
