using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  [CreateAssetMenu (menuName = "GameData/Craft")]
  public class CraftData : ScriptableObject
  {
    public ProductItemData ProductItemData;  
    public MaterialItemData[] Materials;
  }
