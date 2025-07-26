using UnityEngine;
using EditorAttributes;

namespace SHG
{
  public class SceneIdGenerator: MonoBehaviour
  {
    [SerializeField]
    int nextSceneId = 0;

    [Button]
    void Generate()
    {
      var allSmithingTools = GameObject.FindObjectsByType<SmithingToolComponent>(FindObjectsSortMode.None);
      foreach (var smithingTool in allSmithingTools) {
        smithingTool.SceneId = this.nextSceneId++;
      }
    }
  }
}
