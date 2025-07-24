using UnityEngine;

namespace SHG
{
  public class AnvilComponent : MonoBehaviour, IInteractable
  {
    [SerializeField]
    Anvil anvil;
    [SerializeField]
    SmithingToolData data;

    public ToolInteractArgs Interact(PlayerInteractArgs args)
    {
      Debug.Log("Interact");
      return (this.anvil.Interact(args));
    }

    public bool IsInteractable(PlayerInteractArgs args)
    {
      bool isInteractable = this.anvil.IsInteractable(args);
      Debug.Log($"IsInteractable: {isInteractable}");
      return (isInteractable);
    }

    void BeforeInteract(SmithingTool tool, PlayerInteractArgs args)
    {
      Debug.Log("BeforeInteract args");
      Debug.Log(args);
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void AfterInteract(SmithingTool tool, ToolInteractArgs result)
    {
      Debug.Log("AfterInteract result");
      Debug.Log(result);
      Debug.Log($"tool holding item: {tool.HoldingItem}");
      Debug.Log($"tool interaction count: {tool.RemainingInteractionCount}");
    }

    void Awake()
    {
      this.anvil = new Anvil(this.data);
      this.anvil.BeforeInteract += this.BeforeInteract;
      this.anvil.AfterInteract += this.AfterInteract;
    }

    // Update is called once per frame
    void Update()
    {
      this.anvil.OnUpdate(Time.deltaTime);
    }
  }
}
