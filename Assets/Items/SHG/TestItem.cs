using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{

  public class TestItemData: ScriptableObject
  {
    public string Name;
    public GameObject prefab;
    public Sprite Image;
  }

  public class TestItem
  {
    public TestItemData Data;
    public TestItem(TestItemData data)
    {
      this.Data = data;
    }
  }
}
