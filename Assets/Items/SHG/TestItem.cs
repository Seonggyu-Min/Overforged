using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{

  public class ItemData: ScriptableObject
  {
    public string Name;
    public GameObject prefab;
    public Sprite Image;
  }

  public class TestItem
  {
    public ItemData Data;
    public TestItem(ItemData data)
    {
      this.Data = data;
    }
  }
}
