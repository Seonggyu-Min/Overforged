using System;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public struct SoundFile
  {
     public AudioClip Clip;
     [Range(0f, 1f)]
     public float Volume;

     public SoundFile(AudioClip clip, float volume = 1f)
     {
       this.Clip = clip;
       this.Volume = volume;
     }
  }

  [Serializable]
  public struct SoundSource
  {
    public string Name;
    public SoundFile[] SoundFiles;
  }
}
