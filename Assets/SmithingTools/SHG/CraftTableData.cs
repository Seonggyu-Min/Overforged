using UnityEngine;
using EditorAttributes;
using Void = EditorAttributes.Void;

namespace SHG
{
  using CraftData = TestCraftData;
  [CreateAssetMenu (menuName = "GameData/CraftTableData")]
  public class CraftTableData : SmithingToolData
  {
    public CraftData[] CraftList => this.craftList;

    [SerializeField, Validate("Invalid craft data", nameof(HasInvalidCraftData), MessageMode.Error, buildKiller: true)]
    CraftData[] craftList;
    [SerializeField, Validate("No Craft list", nameof(NoCraftList), MessageMode.Error)]
    Void emptyCraftListError;

    protected bool NoCraftList() => (
      this.CraftList == null || this.CraftList.Length == 0);
    protected override bool NoMaterialType() => (false);
    protected bool HasInvalidCraftData()
    {
      if (this.NoCraftList()) {
        return (false);
      }
      foreach (var craft in this.CraftList) {
        if (craft == null) {
          return (true);
        } 
      }
      return (false);
    }
  }
}
