using UnityEngine;

namespace SHG
{
  public static class Utils
  {
    public static T RecursiveFindChild<T>(Transform parent) where T: Component
    {
      T found = null;
      foreach (Transform child in parent) {
        found = child.GetComponent<T>();
        if(found != null) {
          return (found);
        }
        else {
          found = RecursiveFindChild<T>(child);
          if (found != null) {
            return (found);
          }
        }
      }
      return (null);
    }
  }
}
