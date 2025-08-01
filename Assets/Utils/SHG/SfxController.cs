using System;
using UnityEngine;

public class SfxController : MonoBehaviour, IPooledObject
{
  static readonly System.Random VOLUME_RAND = new ();
  public Action<IPooledObject> OnDisabled { get; set; }
  public AudioSource AudioSource { get; private set; }
  public bool IsPlaying => this.AudioSource.clip != null && this.AudioSource.isPlaying;

  protected float remainingPlayTime;

  public SfxController PlaySound(AudioClip clip, Vector3 position, (int min, int max) volumeRange)
  {
    var volume = VOLUME_RAND.Next(
      volumeRange.min,
      volumeRange.max
      );
    this.transform.position = position;
    this
      .SetVolume(volume)
      .PlaySound(clip);
    return (this);
  }

  public SfxController PlaySound(AudioClip clip)
  {
    this
      .SetSound(clip)
      .AudioSource.Play();
    return (this);
  }

  public SfxController Pause()
  {
    this.AudioSource.Pause();
    return (this);
  }

  public SfxController PlayBack()
  {
    if (!this.AudioSource.isPlaying 
      && this.AudioSource.clip != null &&
      this.AudioSource.time != 0) {
      this.AudioSource.UnPause();
    }
    else {
      this.AudioSource.Play();
    }
    return (this);
  }

  public SfxController Stop()
  {
    this.AudioSource.Stop();
    return (this);
  }

  public SfxController SetPosition(Vector3 position)
  {
    this.transform.position = position;
    return (this);
  }

  public SfxController SetVolume(float volume)
  {
    this.AudioSource.volume = volume;
    return (this);
  }

  public SfxController SetSound(AudioClip clip)
  {
    this.AudioSource.Stop();
    this.AudioSource.clip = clip;
    this.remainingPlayTime = clip.length;
    return (this);
  }

  public SfxController SetLoop(bool loop)
  {
    this.AudioSource.loop = loop;
    return (this);
  }

  void Awake()
  {
    this.AudioSource = this.GetComponent<AudioSource>();
  }

  // Update is called once per frame
  protected virtual void Update()
  {
    if (this.AudioSource.loop ||
      !this.IsPlaying)  {
      return ;
    }
    this.remainingPlayTime -= Time.deltaTime;
    if (this.remainingPlayTime <= 0) {
      this.AudioSource.Stop();
      this.AudioSource.clip = null;
      this.transform.position = Vector3.zero;
      this.gameObject.SetActive(false);
    }
  }

  void OnDisable()
  {
    this.AudioSource.loop = false;
    if (this.OnDisabled != null) {
      this.OnDisabled.Invoke(this);
    }
  }
}
