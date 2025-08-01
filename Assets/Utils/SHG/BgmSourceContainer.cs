using System;
using UnityEngine;
using Zenject;

public class BgmSourceContainer : MonoBehaviour
{
  [Inject]
  IAudioLibrary audioLibrary;

  public BgmSource[] BgmSources;

  void Awake()
  {
    if (this.BgmSources == null) {
      return ;
    }
    if (Array.FindIndex(this.BgmSources,
        (soundSource) => string.IsNullOrEmpty(soundSource.Name)) != -1) {
#if UNITY_EDITOR
      Debug.LogError($"{nameof(BgmSourceContainer)} has {nameof(BgmSource)} no name");
#endif
      return ;
    }
    this.audioLibrary.Register(this);
  }
}
