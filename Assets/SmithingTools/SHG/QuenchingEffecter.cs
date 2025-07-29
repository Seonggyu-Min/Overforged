using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class QuenchingEffecter : ISmithingToolEffecter
  {
    public bool[] EffectStates { get; private set; }
    ToonWater toonWater;
    Transform materialPoint;

    public QuenchingEffecter(
      QuenchingTool quenchingTool,
      ToonWater toonWater,
      Transform materialPoint)
    {
      this.toonWater = toonWater;
      this.materialPoint = materialPoint;
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
      this.toonWater.Splash(this.materialPoint);
    }
  }
}
