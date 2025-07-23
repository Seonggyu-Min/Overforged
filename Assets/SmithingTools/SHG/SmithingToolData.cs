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
    const float MAX_REQUIRED_TIME_IN_SECONDS = 10000;
    const int MAX_REQUIRED_INTERACT_COUNT = 20;

    [HideInInspector]
    public string Name => this.toolName;
    [HideInInspector]
    public GameObject Prefab => this.prefab;
    [HideInInspector]
    public MaterialType[] AllowdMaterials;
    [HideInInspector]
    public float TimeRequiredInSeconds => this.timeRequiredInSeconds;
    [HideInInspector]
    public int RequiredInteractCount => this.requiredInteractCount;

    [SerializeField] [Validate("Name is empty", nameof(IsNameEmpty), MessageMode.Error)]
    string toolName;
    [SerializeField] [AssetPreview(64f, 64f), Validate("No prefab", nameof(IsPrefabNone), MessageMode.Error, buildKiller: true)]
    GameObject prefab;
    [SerializeField] [Validate("No material", nameof(EmptyMaterialType), MessageMode.Error, buildKiller:true)]
    MaterialType[] allowdMaterialTypes;
    [SerializeField] [Validate("Invalid required time", nameof(HasValidTimeRequired), MessageMode.Error, buildKiller: true)]
    float timeRequiredInSeconds;
    [SerializeField] [Validate("Invalid required count", nameof(HasValidCountRequired), MessageMode.Error, buildKiller: true)]
    int requiredInteractCount;

    protected bool IsNameEmpty() => (
      this.Name == null || this.Name .Replace(" ", "").Length == 0);
    protected bool IsPrefabNone() => (this.Prefab == null);
    protected bool EmptyMaterialType() => (
      this.AllowdMaterials == null || this.AllowdMaterials.Length == 0);
    protected bool HasValidTimeRequired => (
      this.TimeRequiredInSeconds > 0 && 
      this.TimeRequiredInSeconds < MAX_REQUIRED_TIME_IN_SECONDS
      );
    protected bool HasValidCountRequired => (
      this.RequiredInteractCount > 0 &&
      this.RequiredInteractCount < MAX_REQUIRED_INTERACT_COUNT);
  }
}
