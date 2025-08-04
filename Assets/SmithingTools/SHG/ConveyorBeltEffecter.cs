using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class ConveyorBeltEffecter : ISmithingToolEffecter
  {
    public bool[] EffectStates { get; private set; }

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
