using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SHG
{
  public class SingletonAudio : MonoBehaviour, IAudioLibrary
  {
    public static SingletonAudio Instance => instance;
    static SingletonAudio instance;
    const float DEFAULT_MASTER_VOLUME = 0.5f;
    const float DEFAULT_SFX_VOLUME = 1f;
    const float DEFAULT_BGM_VOLUME = 1f;
    public BgmController BgmController => this.bgmController;
    private AudioMixer mainMixer;
    System.Random rand;
    const int DEFAULT_SFX_POOL_SIZE = 10;
    GameObject sfxControllerPrefab;
    ObjectPool<SfxController> sfxPool;
    Dictionary<string, SoundSource> soundSources;
    List<BgmSource> bgmSources;
    BgmController bgmController;
    HashSet<SfxController> playingAllSfx;
    float[] volumes;
    int currentBgmIndex = -1;

    [RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
      var gameObject = GameObject.Instantiate(Resources.Load<GameObject>("SHG/SingletonAudio"));
      DontDestroyOnLoad(gameObject);
    }

    void Awake()
    {
      if (instance != null) {
        Destroy(this.gameObject);
        return;
      }
      instance = this;
      this.mainMixer = Resources.Load<AudioMixer>("SHG/MainMixer");
      var sfxOutput = this.mainMixer.FindMatchingGroups("Sfx")[0];
      this.rand = new();
      this.soundSources = new ();
      this.bgmSources = new ();
      this.sfxControllerPrefab = Resources.Load<GameObject>("SHG/SfxController");
      this.sfxPool = new MonoBehaviourPool<SfxController>(
        poolSize: DEFAULT_SFX_POOL_SIZE,
        prefab: this.sfxControllerPrefab,
        initPooledObject: (sfxController) =>  {
        sfxController.OnDisabled += this.OnSfxDisabled;
        sfxController.AudioSource.outputAudioMixerGroup = sfxOutput;
        });
      this.playingAllSfx = new ();
      this.volumes = new float[Enum.GetValues(typeof(IAudioLibrary.VolumeType)).Length];
      this.bgmController = GameObject.Instantiate(
        Resources.Load<GameObject>("SHG/BgmController"))
        .GetComponent<BgmController>();
      this.BgmController.AudioSource.outputAudioMixerGroup = this.mainMixer.FindMatchingGroups("Bgm")[0];
      this.SetVolume(IAudioLibrary.VolumeType.Master, DEFAULT_MASTER_VOLUME);
      this.SetVolume(IAudioLibrary.VolumeType.Bgm, DEFAULT_BGM_VOLUME);
      this.SetVolume(IAudioLibrary.VolumeType.Sfx, DEFAULT_SFX_VOLUME);
    }

    void OnDestroy()
    {
      if (instance == this) {
        instance = null;
      }
    }

    public void Register(BgmSourceContainer bgmSource)
    {
      foreach (var bgm in bgmSource.BgmSources) {
        if (this.bgmSources.FindIndex(registeredBgm => registeredBgm.Name == bgm.Name) != -1) {
        #if UNITY_EDITOR
          throw (new ApplicationException($"{nameof(AudioLibrary)}: fail to {nameof(Register)} with duplicated Name {bgm.Name}"));
        #endif
        }
        else {
          this.bgmSources.Add(bgm);
        }
      }
      if (this.currentBgmIndex == -1 && this.bgmSources.Count > 0) {
        this.currentBgmIndex = 0;
        this.PlayBgm(this.bgmSources[this.currentBgmIndex].Name);
      }
    }

    public void Register(SoundSourceContainer audioSource)
    {
      foreach (var soundSource in audioSource.SoundSources) {
        if (!this.soundSources.TryAdd(soundSource.Name, soundSource)) {
        #if UNITY_EDITOR
          throw (new ApplicationException($"{nameof(AudioLibrary)}: fail to {nameof(Register)} with duplicated Name {soundSource.Name}"));
        #endif
        }
      }
      if (this.currentBgmIndex == -1 && this.bgmSources.Count > 0) {
        this.PlayBgm(this.currentBgmIndex);
      }
    }

    public void UnRegister(BgmSourceContainer bgmSource)
    {
      bool isPlaying = this.currentBgmIndex != -1 &&
        bgmSource.BgmSources.Contains(this.bgmSources[this.currentBgmIndex]);
      this.bgmSources = this.bgmSources.Where(
        bgm => !bgmSource.BgmSources.Contains(bgm)).ToList();
      if (!isPlaying) {
        return ;
      }
      if (this.bgmSources.Count > 0) {
        this.currentBgmIndex = 0;
        this.PlayBgm(this.currentBgmIndex);
      }
      else {
        this.currentBgmIndex = -1;
        this.BgmController.StopFadeOut();
      }
    }

    public string GetCurrentBgm()
    {
      if (this.currentBgmIndex > 0) {
        return (this.bgmSources[this.currentBgmIndex].Name);
      }
      return (string.Empty);
    }

    public float GetVolume(IAudioLibrary.VolumeType volumeType)
    {
      return (this.volumes[(int)volumeType]);
    }

    public void SetVolume(IAudioLibrary.VolumeType volumeType, float volume)
    {
      float clampedVolume = Mathf.Clamp(
        volume, 
        0.000001f, 1f);
      float dbVolume = Mathf.Log10(clampedVolume) * 20f;
      this.volumes[(int)volumeType] = clampedVolume;
      switch (volumeType) {
        case IAudioLibrary.VolumeType.Master:
          this.mainMixer.SetFloat("MasterVolume", dbVolume);
          break;
        case IAudioLibrary.VolumeType.Bgm:
          this.mainMixer.SetFloat("BgmVolume", dbVolume);
          break;
        case IAudioLibrary.VolumeType.Sfx:
          this.mainMixer.SetFloat("SfxVolume", dbVolume);
          break;
      }
    }

    public void Mute()
    {
      this.SetVolume(IAudioLibrary.VolumeType.Master, 0f);
    }

    public void PlayBgm(string bgmName)
    {
      int foundIndex = this.bgmSources.FindIndex(
        bgmSource => bgmSource.Name == bgmName); 
      if (foundIndex != -1) {
        this.currentBgmIndex = foundIndex;
        this.PlayBgm(this.bgmSources[foundIndex].SoundFile.Clip);
      }
    }

    public void PlaySfx(AudioClip clip)
    {
      this
        .GetSfxController()
        .PlaySound(clip);
    }

    public void PauseBgm()
    {
      if (this.BgmController.IsPlaying) {
        this.BgmController.Pause(); 
      }
    }

    public void PauseAllSfx()
    {
      foreach (var sfxController in this.playingAllSfx) {
        sfxController.Pause(); 
      }
    }

    public void PlaybackBgm()
    {
      this.BgmController.PlayBack();
    }

    public void PlaybackAllSfx()
    {
      foreach (var sfxController in this.playingAllSfx) {
        sfxController.PlayBack();
      }
    }

    public void PlayNextBgm()
    {
      int index = this.currentBgmIndex < this.bgmSources.Count - 1 ?
        this.currentBgmIndex + 1: 0;
      this.currentBgmIndex = index;
      this.PlayBgm(this.bgmSources[index].SoundFile.Clip);
    }

    public string[] GetBgmList()
    {
      string[] list = new string[this.bgmSources.Count];
      for (int i = 0; i < list.Length; i++) {
        list[i] = this.bgmSources[i].Name; 
      }
      return (list);
    }

    public SfxController GetSfxController()
    {
      return (this.sfxPool.Get());
    }

    public SfxController PlayRandomSfx(
      in string soundSourceName,
      Nullable<Vector3> position = null)
    {
      if (!this.soundSources.TryGetValue(
          soundSourceName, out SoundSource soundSource)) {
        #if UNITY_EDITOR
        Debug.LogError($"{nameof(GetRandomSoundFrom)}: Fail to find {nameof(SoundSource)} for Name {soundSourceName}");
        #endif
        return (null);
      }
      SoundFile sound = this.GetRandomSoundFrom(soundSource);
      return (this.PlaySfx(sound.Clip, position, sound.Volume));
    }

    public SfxController PlaySfx(
      AudioClip clip,
      Nullable<Vector3> position = null, 
      float volume = 1f)
    {
     var sfxController = this.GetSfxController()
       .SetPosition(position ?? Camera.main.transform.position)
       .SetVolume(volume)
       .PlaySound(clip);
     this.playingAllSfx.Add(sfxController);
     return (sfxController);
    }

    void OnSfxDisabled(IPooledObject pooledObject)
    {
      if (pooledObject is SfxController sfxController) {
        this.playingAllSfx.Remove(sfxController);
      }
    }

    void PlayBgm(int index) 
    {
      this.PlayBgm(this.bgmSources[index].SoundFile.Clip);
    }

    void PlayBgm(AudioClip clip)
    {
      if (this.BgmController.IsPlaying) {
        this.BgmController.CrossFadeBgm(clip);
      }
      else {
        this.BgmController.PlaySound(clip);
      }
    }

    SoundFile GetRandomSoundFrom(SoundSource soundSource)
    {
      if (soundSource.SoundFiles.Length > 1)
      {
        int index = this.rand.Next(0, soundSource.SoundFiles.Length);
        return (soundSource.SoundFiles[index]);
      }
      else {
        return (soundSource.SoundFiles[0]);
      }
    }
  }
}
