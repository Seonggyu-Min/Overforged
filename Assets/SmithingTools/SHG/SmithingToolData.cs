using System;
using UnityEngine;
using EditorAttributes;
using Void = EditorAttributes.Void;

namespace SHG
{

  [Serializable]
  [CreateAssetMenu(menuName = "GameData/SmithingToolData")]
  public class SmithingToolData : ScriptableObject
  {
    const float MAX_REQUIRED_TIME_IN_SECONDS = 10000;
    const int MAX_REQUIRED_INTERACT_COUNT = 20;
    const float MAX_INTERACTION_TIME_IN_SECOND = 30;

    [HideInInspector]
    public string Name => this.toolName;
    [HideInInspector]
    public GameObject Prefab => this.prefab;
    [HideInInspector]
    public MaterialVariation[] AllowedMaterials => this.allowdMaterialTypes;
    [HideInInspector]
    public float TimeRequiredInSeconds => this.timeRequiredInSeconds;
    [HideInInspector]
    public int RequiredInteractCount => this.requiredInteractCount;
    [HideInInspector]
    public float InteractionTime => this.interactionTime;

    [SerializeField] [Validate("Name is empty", nameof(IsNameEmpty), MessageMode.Error)]
    string toolName;
    [SerializeField] [AssetPreview(64f, 64f), Validate("No prefab", nameof(IsPrefabNone), MessageMode.Error, buildKiller: true)]
    GameObject prefab;
    [SerializeField] [ReadOnly, Validate("No material", nameof(NoMaterialType), MessageMode.Error, buildKiller: true)]
    Void noMaterialCheck;
    [SerializeField]
    MaterialVariation[] allowdMaterialTypes;
    [SerializeField] [Validate("Invalid required time", nameof(HasInValidTimeRequired), MessageMode.Error, buildKiller: true)]
    float timeRequiredInSeconds;
    [SerializeField] [Validate("Invalid interation time", nameof(HasInValidInteractionTime), MessageMode.Error, buildKiller: true)]
    float interactionTime;
    [SerializeField] [Validate("Invalid required count", nameof(HasInValidCountRequired), MessageMode.Error, buildKiller: true)]
    int requiredInteractCount;

    protected bool IsNameEmpty() => (
      this.Name == null || this.Name .Replace(" ", "").Length == 0);
    protected bool IsPrefabNone() => (this.Prefab == null);
    protected bool NoMaterialType() => (
      this.AllowedMaterials == null || this.AllowedMaterials.Length == 0);
    protected bool HasInValidTimeRequired => (
      this.TimeRequiredInSeconds < 0 ||
      this.TimeRequiredInSeconds > MAX_REQUIRED_TIME_IN_SECONDS
      );
    protected bool HasInValidInteractionTime => (
      this.TimeRequiredInSeconds < 0 ||
      this.TimeRequiredInSeconds > MAX_INTERACTION_TIME_IN_SECOND
      );
    protected bool HasInValidCountRequired => (
      this.RequiredInteractCount <= 0 ||
      this.RequiredInteractCount > MAX_REQUIRED_INTERACT_COUNT);
  }
}
