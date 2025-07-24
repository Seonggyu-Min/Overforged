using System;
using UnityEngine;
using Zenject;
using EditorAttributes;
using Photon.Pun;

namespace SHG
{
  public class ZenjectTestObject : MonoBehaviour
  {

    [Inject]
    public IGameManager GameManager;


    [Inject]
    public void Construct(IGameManager gameManager)
    {
      this.GameManager = gameManager;
    }

    void Start()
    {
      Debug.Log($"game manager: {this.GameManager}");
    }

    [Button]
    void PrintGameVersion()
    {
      var version = this.GameManager.GetVersion();
      Debug.Log($"game version: {version}");
    }

    [Button]
    void ChangeVersion(string version)
    {
      this.GameManager.ChangeVersion(version); 
    }
  }
}
