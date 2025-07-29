using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class TableEffecter : ISmithingToolEffecter
  {
    public bool[] EffectStates { get; private set; }
    WoodTable woodTable;
    CraftTable craftTable;

    public TableEffecter(
      WoodTable woodTable,
      CraftTable craftTable
      )
    {
      this.woodTable = woodTable;
      this.craftTable = craftTable;
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

    }
  }
}
