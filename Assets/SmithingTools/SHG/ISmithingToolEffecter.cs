
namespace SHG
{
  public enum ToolEffectState
  {
    HoldingItem,
    Working,
    Finished
  }

  public interface ISmithingToolEffecter<T> where T: SmithingTool
  {
    public bool[] EffectStates { get; }
    public bool IsStateOn(ToolEffectState state);
    public void TurnOnState(ToolEffectState newState);
    public void TriggerWorkEffect();
    public void OnUpdate(float deltaTime);
  }
}
