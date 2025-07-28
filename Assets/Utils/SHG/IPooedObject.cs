using System;

namespace SHG
{
  public interface IPooledObject 
  {
    public Action<IPooledObject> OnDisabled { get; set; }
  }
}
