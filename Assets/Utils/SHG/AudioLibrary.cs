using System;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public interface IAudioLibrary
  {
    SoundFile GetRandomSoundFrom(SoundSource soundSource);
    SfxController GetSfxController();
    SfxController PlayRandomSound(in string soundName, Vector3? position = null, float? volume = null);
    SfxController PlaySfx(AudioClip clip, Vector3? position = null, float? volume = null);
    void Register(SoundSourceContainer audioSource);
  }

  public class AudioLibrary : IAudioLibrary
  {
    System.Random rand;
    const int DEFAULT_SFX_POOL_SIZE = 10;
    GameObject sfxControllerPrefab;
    ObjectPool<SfxController> sfxPool;
    Dictionary<string, SoundSource> soundSources;

    public AudioLibrary()
    {
      this.rand = new();
      this.soundSources = new();
      this.sfxControllerPrefab = Resources.Load<GameObject>("SHG/SfxController");
      this.sfxPool = new MonoBehaviourPool<SfxController>(
        poolSize: DEFAULT_SFX_POOL_SIZE,
        prefab: this.sfxControllerPrefab);
    }

    public void Register(SoundSourceContainer audioSource)
    {
      foreach (var soundSource in audioSource.SoundSources)
      {
        if (!this.soundSources.TryAdd(soundSource.Name, soundSource))
        {
#if UNITY_EDITOR
          throw (new ApplicationException($"{nameof(AudioLibrary)}: fail to {nameof(Register)} with duplicated Name {soundSource.Name}"));
#endif
        }
      }
    }

    public SoundFile GetRandomSoundFrom(SoundSource soundSource)
    {
      if (soundSource.SoundFiles.Length > 1)
      {
        int index = this.rand.Next(0, soundSource.SoundFiles.Length);
        return (soundSource.SoundFiles[index]);
      }
      else
      {
        return (soundSource.SoundFiles[0]);
      }
    }

    public SfxController GetSfxController()
    {
      return (this.sfxPool.Get());
    }

    public SfxController PlayRandomSound(in string soundSourceName,
      Nullable<Vector3> position = null, Nullable<float> volume = null)
    {
      if (!this.soundSources.TryGetValue(
          soundSourceName, out SoundSource soundSource))
      {
#if UNITY_EDITOR
        Debug.LogError($"{nameof(GetRandomSoundFrom)}: Fail to find {nameof(SoundSource)} for Name {soundSourceName}");
#endif
        return (null);
      }
      SoundFile sound = this.GetRandomSoundFrom(soundSource);
      float multiplyedVolume = (volume ?? 1.0f) * sound.Volume;
      return (this.PlaySfx(sound.Clip, position, multiplyedVolume));
    }

    public SfxController PlaySfx(AudioClip clip, Nullable<Vector3> position = null, Nullable<float> volume = null)
    {
     return (
       this.GetSfxController()
       .SetPosition(position ?? Camera.main.transform.position)
       .SetVolume(volume ?? 1.0f)
       .PlaySound(clip));
    }
  }
}
