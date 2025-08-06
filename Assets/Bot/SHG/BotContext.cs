using System;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using Photon.Pun;

namespace SHG
{
  using ConveyComponent = NewProductConveyComponent;
  public class BotContext : MonoBehaviour
  {
    public static BotContext Instance => instance;
    static BotContext instance;
    [SerializeField]
    RawMaterialBox[] materialBoxes;
    public RawMaterialBox[] MaterialBoxes => this.materialBoxes;
    Dictionary<int, List<SmithingToolComponent>> tools;
    [SerializeField]
    List<ProductRecipe> recipes;
    public Action<ProductRecipe> OnRecipeAdded;
    public Action<ProductRecipe> OnRecipeRemoved;
    public List<ConveyComponent> submitPlaces;

    public void AddRecipe(
      ProductItemData data,
      WoodType woodType,
      OreType oreType)
    {
      var recipe = new ProductRecipe(
          productType: data.productType,
          oreType: oreType,
          woodType: woodType,
          timeStamp: Time.time);
      this.recipes.Add(recipe);
      this.OnRecipeAdded?.Invoke(recipe);
    }

    public void RemoveRecipe(
      ProductItemData data,
      WoodType woodType,
      OreType oreType)
    {
      int index = this.recipes.FindIndex(
        recipe => recipe.IsEqualTo(
          productType: data.productType,
          oreType: oreType,
          woodType: woodType));
      if (index != -1) {
        var recipe = this.recipes[index];
        this.recipes.RemoveAt(index);
        this.OnRecipeRemoved?.Invoke(recipe);
      }
    }

    public bool TryGetNextRecipe(out ProductRecipe recipe)
    {
      if (this.recipes.Count > 0) {
        recipe = this.recipes[0];
        return (true);
      }
      recipe = new ProductRecipe{};
      return (false);
    }

    public bool IsValidRecipe(in ProductRecipe recipe)
    {
      for (int i = 0; i < this.recipes.Count; i++) {
        if (this.recipes[i].Equals(recipe)) {
          return (true);
        }
      }
      return (false);
    }

    public void AddTool(SmithingToolComponent tool)
    {
      if (!this.tools.ContainsKey(tool.PlayerNetworkId)) {
        this.tools.Add(tool.PlayerNetworkId, new List<SmithingToolComponent>()); 
      }
      this.tools[tool.PlayerNetworkId].Add(tool);
    }

    [Button]
    public void SpawnBot(Vector3 position, int playerNetworkId)
    {
      var botObject = PhotonNetwork.Instantiate(
        "SHG/Bot",
        position: position,
        rotation: Quaternion.identity);
      var botController = botObject.GetComponent<EnemyBotController>();
      bool isOwner = PhotonNetwork.LocalPlayer.IsMasterClient;
      if (botController != null && isOwner) {
        botController.IsOwner = isOwner;
        botController.StartCreateProduct();
      }
    }

    public bool TryGetClosestSubmitPlace(Vector3 position, out ConveyComponent submitPlace)
    {
      if (this.submitPlaces.Count == 1) {
        submitPlace = this.submitPlaces[0];
        return (true);
      }
      submitPlace = null;
      if (this.submitPlaces.Count > 1) {
        float dist = float.MaxValue;
        foreach (var place in this.submitPlaces) {
          float curDist = Vector3.Distance(
            place.transform.position,
            position); 
          if (curDist < dist) {
            submitPlace = place;
          }
        }
        return (true);
      }
      return (false);

    }
    
    void Awake()
    {
      if (instance == null) {
        instance = this;
        this.Init();
      }
      else {
        Destroy(this.gameObject);
      }
    }

    void Start()
    {
      foreach (var interactObject in GameObject.FindGameObjectsWithTag(
          "InteractionObj")) {
        var convey = interactObject.GetComponent<ConveyComponent>();
        if (convey != null) {
          this.submitPlaces.Add(convey);
        }
      }
    }

    void Init()
    {
      this.tools = new ();
      this.recipes = new (); 
      this.submitPlaces = new ();
    }

    public T GetComponent<T>(int networkId, SmithingTool.ToolType toolType) where T: SmithingToolComponent
    {
      if (this.tools.TryGetValue(
          networkId, out List<SmithingToolComponent> botTools)) {
        switch (toolType) {
          case (SmithingTool.ToolType.Furnace):
            return (botTools.Find(tool => tool is FurnaceComponent) as T);
          case (SmithingTool.ToolType.Anvil):
            return (botTools.Find(tool => tool is AnvilComponent) as T);
          case (SmithingTool.ToolType.WoodTable):
          case (SmithingTool.ToolType.CraftTable):
            return (botTools.Find(tool => tool is TableComponent) as T);
          case (SmithingTool.ToolType.QuenchingTool):
            return (botTools.Find(tool => tool is QuenchingComponent) as T);
          default: 
            return (null as T);
        } 
      }
      else {
        return (null as T);
      }
    }

    void OnDestroy()
    {
      if (instance == this) {
        instance = null;
      }
    }

    #region TestCode
    #if UNITY_EDITOR
    [SerializeField] [VerticalGroup(10f, true, nameof(craftData), nameof(woodType), nameof(oreType))]
    EditorAttributes.Void testGroup;
    [SerializeField] [HideInInspector]
    ProductItemData craftData;
    [SerializeField] [HideInInspector]
    WoodType woodType;
    [SerializeField] [HideInInspector]
    OreType oreType;

    [Button] 
    void AddRecipeTest()
    {
      this.AddRecipe(this.craftData, this.woodType, this.oreType);
    }

    [Button]
    void RemoveRecipeTest()
    {
      this.RemoveRecipe(this.craftData, this.woodType, this.oreType);
    }
    #endif
    #endregion
  }
}
