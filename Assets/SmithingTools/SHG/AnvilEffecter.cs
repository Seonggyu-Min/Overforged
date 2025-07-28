using System;
using UnityEngine;

namespace SHG
{
  public class AnvilEffecter : ISmithingToolEffecter
  {
    public bool[] EffectStates { get; private set; }
    Anvil anvil;
    MonoBehaviourPool<SimplePooledObject> sparkPool;
    Transform sparkPoint;

    public AnvilEffecter( 
      Anvil anvil,
      MonoBehaviourPool<SimplePooledObject> sparkPool,
      Transform sparkPoint)
    {
      this.anvil = anvil;
      this.sparkPool = sparkPool;
      this.sparkPoint = sparkPoint;
      this.EffectStates = new bool[
        Enum.GetValues(typeof(ISmithingToolEffecter.State)).Length
      ];
    }

    public bool IsStateOn(ISmithingToolEffecter.State state)
    {
      return (this.EffectStates[(int)state]);
    }

    public void OnUpdate(float deltaTime)
    {

    }

    public void TriggerWorkEffect()
    {
      var spark = this.sparkPool.Get();
      spark.transform.position = this.sparkPoint.position;
      spark.transform.forward = this.sparkPoint.forward;
      spark.gameObject.SetActive(true);
    }

    public void ToggleState(ISmithingToolEffecter.State state)
    {
      this.EffectStates[(int)state] = !this.EffectStates[(int)state];
    }
  }
}
