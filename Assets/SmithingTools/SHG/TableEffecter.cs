using System;
using UnityEngine;

namespace SHG
{
  public class TableEffecter : ISmithingToolEffecter
  {
    public bool[] EffectStates { get; private set; }
    WoodTable woodTable;
    CraftTable craftTable;
    Func<IInteractableTool> getCurrentTool;
    ParticleSystem sawDustParticleSystem;
    ParticleSystem confettiParticleSystem;

    public TableEffecter(
      WoodTable woodTable,
      CraftTable craftTable,
      ParticleSystem sawDustParticleSystem,
      ParticleSystem confettiParticleSystem,
      Func<IInteractableTool> getCurrentTool
      )
    {
      this.woodTable = woodTable;
      this.craftTable = craftTable;
      this.sawDustParticleSystem = sawDustParticleSystem;
      this.confettiParticleSystem = confettiParticleSystem;
      this.getCurrentTool = getCurrentTool;
    }

    public bool IsStateOn(ISmithingToolEffecter.State state)
    {
      return (this.EffectStates[(int)state]);
    }

    public void OnUpdate(float deltaTime)
    {

    }

    public void ToggleState(ISmithingToolEffecter.State state)
    {
      this.EffectStates[(int)state] = !this.EffectStates[(int)state];

    }

    public void TriggerWorkEffect()
    {
      var currentTool = this.getCurrentTool();
      if (currentTool is WoodTable) {
        this.sawDustParticleSystem.Clear();
        this.sawDustParticleSystem.Play();
      }
      else if (currentTool is CraftTable) {
        this.confettiParticleSystem.Clear();
        this.confettiParticleSystem.Play();
      }
    }
  }
}
