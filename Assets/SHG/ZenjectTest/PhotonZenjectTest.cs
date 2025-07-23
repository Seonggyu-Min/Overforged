using System;
using UnityEngine;

namespace SHG
{

  public interface IGameManager
  {
    public string GetVersion();
    public void ChangeVersion(string version);
  }

  [Serializable]
  public class GameManager: IGameManager
  {
    [SerializeField]
    string currentVersion = "1.0.0";

    public string GetVersion()
    {
      return (this.currentVersion);
    }

    public void ChangeVersion(string version)
    {
      Debug.Log($"Version change: {version}");
      this.currentVersion = version;
    }
  }
}
