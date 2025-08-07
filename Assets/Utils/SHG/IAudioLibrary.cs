using UnityEngine;

public interface IAudioLibrary
{
  public enum VolumeType
  {
    Master,
    Bgm,
    Sfx
  }
  public void Mute();
  public float GetVolume(VolumeType volumeType);
  public void SetVolume(VolumeType volumeType, float volume);
  public void PlaySfx(AudioClip clip);
  public void PauseBgm();
  public void PauseAllSfx();
  public void PlaybackBgm();
  public void PlaybackAllSfx();
  SfxController PlayRandomSfx(in string soundName, Vector3? position = null);
  SfxController PlaySfx(AudioClip clip, Vector3? position = null, float volume = 1f);
  public void PlayBgm(string bgmName);
  public void PlayNextBgm();
  public void Register(SoundSourceContainer audioSource);
  public void Register(BgmSourceContainer bgmSource);
  public void UnRegister(BgmSourceContainer bgmSource);
  public string[] GetBgmList();
  public string GetCurrentBgm();
}
