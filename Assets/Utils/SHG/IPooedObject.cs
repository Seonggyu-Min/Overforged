using System;

public interface IPooledObject 
{
  public Action<IPooledObject> OnDisabled { get; set; }
}
