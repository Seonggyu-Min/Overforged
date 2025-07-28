using System;
using UnityEngine;

namespace SHG
{
  public class SimplePooledObject : MonoBehaviour, IPooledObject
  {
    public Action<IPooledObject> OnDisabled 
    { 
      get => this.onDisabled as Action<IPooledObject>;
      set => this.onDisabled = value;
    }
    Action<SimplePooledObject> onDisabled;
    public virtual float LifeTime { get; set; }

    void Update()
    {
      this.LifeTime -= Time.deltaTime;
      if (this.LifeTime <= 0) {
        this.gameObject.SetActive(false);
      }
    }

    void OnDisable()
    {
      if (this.onDisabled != null) {
        this.onDisabled.Invoke(this);
      }
      else {
        Destroy(this.gameObject);
      }
    }
  }
}
