using System;
using UnityEngine;

[Serializable]
public struct SoundFile
{
  public float Volume => this.isVolumeModified ? this.volume: 1.0f;
  public AudioClip Clip;
  [SerializeField] [Range(0f, 1f)]
  public float volume;
  [SerializeField]
  bool isVolumeModified;

  public SoundFile(AudioClip clip, float volume = 1f)
  {
    this.Clip = clip;
    this.volume = volume;
    this.isVolumeModified = volume != 1f;
  }
}

[Serializable]
public struct SoundSource
{
  public string Name;
  public SoundFile[] SoundFiles;
}

[Serializable]
public struct BgmSource
{
  public string Name;
  public SoundFile SoundFile;
}
