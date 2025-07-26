using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SHG
{
  #if UNITY_EDITOR
  public static class SceneIdGenerator
  {

    [MenuItem("Edit/SceneIdGenerator/SmithingTool")]
    static void GenerateSmithingToolId()
    {
      int nextSceneId = 0;
      var allSmithingTools = GameObject.FindObjectsByType<SmithingToolComponent>(FindObjectsSortMode.None);
      foreach (var smithingTool in allSmithingTools) {
        var so = new SerializedObject(smithingTool);
        so.FindProperty("id").intValue = nextSceneId++;
        so.ApplyModifiedProperties(); 
      }
    }
  }
  #endif
}
