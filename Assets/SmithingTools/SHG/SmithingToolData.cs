using System;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  using MaterialType = TestMaterialType;

  [Serializable]
  [CreateAssetMenu(menuName = "GameData/SmithingToolData")]
  public class SmithingToolData : ScriptableObject
  {
    [HideInInspector]
    public string Name => this.toolName;
    [HideInInspector]
    public GameObject Prefab => this.prefab;
    [HideInInspector]
    public MaterialType[] AllowdMaterials;

    [SerializeField] [Validate("Name is empty", nameof(IsNameEmpty), MessageMode.Error)]
    string toolName;
    [SerializeField] [AssetPreview(64f, 64f), Validate("No prefab", nameof(IsPrefabNone), MessageMode.Error, buildKiller: true)]
    GameObject prefab;
    [SerializeField] [Validate("No material", nameof(EmptyMaterialType), MessageMode.Error, buildKiller:true)]
    MaterialType[] allowdMaterialTypes;

    protected bool IsNameEmpty() => (
      this.Name == null || this.Name .Replace(" ", "").Length == 0);
    protected bool IsPrefabNone() => (this.Prefab == null);
    protected bool EmptyMaterialType() => (
      this.AllowdMaterials == null || this.AllowdMaterials.Length == 0);
  }
}
