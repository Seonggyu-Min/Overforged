using UnityEngine;

namespace SHG
{
  [CreateAssetMenu (menuName = "GameData/SmithingToolList")]
  public class SmithingToolList : ScriptableObject
  {
    public SmithingToolData[] Tools => this.toolList;
    [SerializeField]
    SmithingToolData[] toolList;
  }
}
