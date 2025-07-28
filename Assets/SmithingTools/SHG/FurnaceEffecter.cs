using System;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public class FurnaceEffecter : ISmithingToolEffecter<Furnace>
  {
    static class Constants
    {
      public const float CHANGE_THRESHOLD = 0.1f;
      public const float MAX_FIRE_START_SPEED = 0.4f;
      public const float MIN_FIRE_START_SPEED = 0.05f;
      public const float MIN_FIRE_START_SIZE = 0.5f;
      public const float MAX_FIRE_START_SIZE = 2.5f;
      public const float MAX_SPARK_START_SPEED = 1.25f;
      public const float MIN_SPARK_START_SPEED = 0.2f;
      public const float MAX_SPARK_START_SIZE = 0.15f;
      public const float MIN_SPARK_START_SIZE = 0.05f;
    }
    ParticleSystem fireParticle;
    ParticleSystem sparkParticle;
    ParticleSystem.MainModule fireParticleModule;
    ParticleSystem.MainModule sparkParticleModule;
    Furnace furnace;
    float lastMagnitude;

    public bool[] EffectStates { get; private set; }

    public FurnaceEffecter(
      Furnace furnace,
      ParticleSystem fireParticle,
      ParticleSystem sparkParticle)
    {
      this.furnace = furnace;
      this.fireParticle = fireParticle;
      this.fireParticleModule = fireParticle.main;
      this.sparkParticle = sparkParticle;
      this.sparkParticleModule = sparkParticle.main;
      this.SetFireMagnitude(0f);
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

    public void SetFireMagnitude(float magnitude)
    {
      this.lastMagnitude = magnitude;
      #if UNITY_EDITOR
      if (magnitude < 0 || magnitude > 1) {
        throw (new ArgumentException($"{nameof(magnitude)} has to between 0 and 1"));
      }
      this.fireParticleModule.startSpeed = Mathf.Lerp(
        Constants.MIN_FIRE_START_SPEED,
        Constants.MAX_FIRE_START_SPEED,
        magnitude);
      this.fireParticleModule.startSize = Mathf.Lerp(
        Constants.MIN_FIRE_START_SIZE,
        Constants.MAX_FIRE_START_SIZE,
        magnitude);
      this.sparkParticleModule.startSpeed = Mathf.Lerp(
        Constants.MIN_SPARK_START_SPEED,
        Constants.MAX_SPARK_START_SPEED,
        magnitude);
      this.sparkParticleModule.startSize = Mathf.Lerp(
        Constants.MIN_SPARK_START_SIZE,
        Constants.MAX_SPARK_START_SIZE,
        magnitude);
      #endif
    }

    public void OnUpdate(float detaTime)
    {
      if (Math.Abs(this.lastMagnitude - this.furnace.NormalizedTemparature) < 
        Constants.CHANGE_THRESHOLD) {
        return ;
      }
      this.SetFireMagnitude(this.furnace.NormalizedTemparature);
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
