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
      var allBehaviours = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
      foreach (var behaviour in allBehaviours) {
        if (behaviour is INetSynchronizable synchronizable) {
          synchronizable.SceneId = this.nextSceneId++;
        } 
      }
    }
  }
}
