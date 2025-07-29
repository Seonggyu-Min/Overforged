using UnityEngine;

namespace SHG
{
  public class QuenchingEffecter : ISmithingToolEffecter
  {
    public bool[] EffectStates { get; private set; }
    ToonWater toonWater;
    Transform materialPoint;
    ParticleSystem vaporParticle;

    public QuenchingEffecter(
      QuenchingTool quenchingTool,
      ToonWater toonWater,
      ParticleSystem vaporParticle,
      Transform materialPoint)
    {
      this.toonWater = toonWater;
      this.materialPoint = materialPoint;
      this.vaporParticle = vaporParticle;
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
      this.vaporParticle.Clear();
      this.toonWater.Splash(this.materialPoint);
      this.vaporParticle.Play();
    }
  }
}
