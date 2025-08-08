using System;
using UnityEngine;
using Zenject;
using SHG;

public class SoundSourceContainer: MonoBehaviour
{
  IAudioLibrary audioLibrary => SingletonAudio.Instance;

  public SoundSource[] SoundSources;
  [SerializeField]
  bool isSingletonSource;

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
    }
    this.audioLibrary.Register(this);
  }
}
