using System;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  [Serializable]
  public class GameObjectHighlighter: IHighlightable
  {
    const float INSTANT_HIGHLIGHTED_DURATION = 0.1f; 
    const string EMISSION_KEYWORD = "_EMISSION";
    [ShowInInspector]
    public bool IsHighlighted { get; private set; }
    [ShowInInspector]
    public Material[] HighlightedMaterials { get; private set; }
    [HideInInspector]

    public Color HighlightColor
    {
      get => this.highlightColor;
      set {
        if (this.highlightColor == value) {
          return ;
        }
        foreach (var material in this.HighlightedMaterials) {
          material.SetColor("_EmissionColor", value);
        }
        this.highlightColor = value;
      }
    }
    [SerializeField]
    Color highlightColor;
    [SerializeField]
    float durationToHighlight;

    public GameObjectHighlighter(Material[] baseMaterials)
    {
      this.HighlightedMaterials = Array.ConvertAll<Material, Material>(
        baseMaterials,
        material => new Material(material));
    }

    public void HighlightForSeconds(float seconds, Color color)
    {
      this.durationToHighlight = seconds;
      this.HighlightColor = color;
      this.IsHighlighted = true;
      foreach (var material in this.HighlightedMaterials) {
        material.EnableKeyword(EMISSION_KEYWORD); 
      }
    }

    public void HighlightInstantly(Color color)
    {
      this.durationToHighlight = Math.Max(
        INSTANT_HIGHLIGHTED_DURATION, this.durationToHighlight);
      this.HighlightColor = color;
      this.IsHighlighted = true;
      foreach (var material in this.HighlightedMaterials) {
        material.EnableKeyword(EMISSION_KEYWORD); 
      }
    }

    public void OnUpdate(float deltaTime)
    {
      if (this.IsHighlighted) {
        this.durationToHighlight -= deltaTime;
        if (this.durationToHighlight < 0) {
          this.IsHighlighted = false;
          foreach (var material in this.HighlightedMaterials) {
            material.DisableKeyword(EMISSION_KEYWORD); 
          }
        }
      }
    }
  }
}
