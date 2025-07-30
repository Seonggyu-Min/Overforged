using System;
using UnityEngine;

namespace SHG
{
  public class SfxController : MonoBehaviour, IPooledObject
  {
    public Action<IPooledObject> OnDisabled { get; set; }
    AudioSource source;
    static readonly System.Random VOLUME_RAND = new ();

    float remainingPlayTime;

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
        .source.Play();
      return (this);
    }

    public SfxController Pause()
    {
      this.source.Pause();
      return (this);
    }

    public SfxController PlayBack()
    {
      if (this.source.isPlaying && this.source.time != 0) {
        this.source.UnPause();
      }
      else {
        this.source.Play();
      }
      return (this);
    }

    public SfxController Stop()
    {
      this.source.Stop();
      return (this);
    }

    public SfxController SetPosition(Vector3 position)
    {
      this.transform.position = position;
      return (this);
    }

    public SfxController SetVolume(float volume)
    {
      this.source.volume = volume;
      return (this);
    }

    public SfxController SetSound(AudioClip clip)
    {
      this.source.Stop();
      this.source.clip = clip;
      this.remainingPlayTime = clip.length;
      return (this);
    }

    public SfxController SetLoop(bool loop)
    {
      this.source.loop = loop;
      return (this);
    }

    void Awake()
    {
      this.source = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      if (this.source.loop)  {
        return ;
      }
      this.remainingPlayTime -= Time.deltaTime;
      if (this.remainingPlayTime <= 0) {
        this.source.Stop();
        this.source.clip = null;
        this.transform.position = Vector3.zero;
        this.gameObject.SetActive(false);
      }
    }

    void OnDisable()
    {
      this.source.loop = false;
      if (this.OnDisabled != null) {
        this.OnDisabled.Invoke(this);
      }
    }
  }


}
