using System;
using UnityEngine;
using Zenject;

public class SoundSourceContainer: MonoBehaviour
{
  [Inject]
  IAudioLibrary audioLibrary;

  public SoundSource[] SoundSources;

  void Awake()
  {
    if (this.SoundSources == null) {
      return ;
    }
    if (Array.FindIndex(this.SoundSources,
        (soundSource) => string.IsNullOrEmpty(soundSource.Name)) != -1) {
#if UNITY_EDITOR
      Debug.LogError($"{nameof(SoundSourceContainer)} has {nameof(SoundSources)} no name");
#endif
      return ;
    }
    if (Array.FindIndex(this.SoundSources, 
        SoundSource => (
          SoundSource.SoundFiles == null ||
          SoundSource.SoundFiles.Length == 0)) != -1) {
#if UNITY_EDITOR
      Debug.LogError($"{nameof(SoundSourceContainer)} has {nameof(SoundSources)} no SoundFiles");
#endif
      return ;
    }
    this.audioLibrary.Register(this);
  }
}
