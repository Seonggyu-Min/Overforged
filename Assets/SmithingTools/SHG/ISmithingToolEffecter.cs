
namespace SHG
{

  public interface ISmithingToolEffecter
  {
    public enum State
    {
      HoldingItem,
      Working,
      Finished
    }
    public bool[] EffectStates { get; }
    public bool IsStateOn(State state);
    public void ToggleState(State state);
    public void TriggerWorkEffect();
    public void OnUpdate(float deltaTime);
  }
}
