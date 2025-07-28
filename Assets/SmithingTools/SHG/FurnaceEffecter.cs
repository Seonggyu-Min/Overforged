using System;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  [Serializable]
  public class FurnaceEffecter : ISmithingToolEffecter<Furnace>
  {
    [SerializeField] [Required(throwValidationError: true)]
    ParticleSystem fireParticle;
    [SerializeField] [Required(throwValidationError: true)]
    ParticleSystem sparkParticle;

    public bool[] EffectStates { get; private set; }

    public FurnaceEffecter(Furnace furnace)
    {

    }

    public void TurnOn()
    {
      this.fireParticle.Play();
      this.sparkParticle.Play();
    }

    public void TurnOff()
    {
      this.fireParticle.Stop();
      this.sparkParticle.Stop();
    }

    public void OnUpdate(float detaTime)
    {

    }

    public bool IsStateOn(ToolEffectState state)
    {
      return (this.EffectStates[(int)state]);
    }

    public void TriggerWorkEffect()
    {

    }

    public void TurnOnState(ToolEffectState newState)
    {
      throw new System.NotImplementedException();
    }
  }
}
