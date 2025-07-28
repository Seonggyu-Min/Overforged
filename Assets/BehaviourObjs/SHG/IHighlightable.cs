using UnityEngine;

namespace SHG
{
  public interface IHighlightable
  {
    public bool IsHighlighted { get; }
    public Color HighlightColor { get; set; }
    public void HighlightInstantly(Color color);     
    public void HighlightForSeconds(float seconds, Color color);
  }
}
