using System;
using System.Collections;
using UnityEngine;

namespace SHG
{
  public class BgmController : SfxController
  {
    const float FADE_VOLUME_DELTA = 0.6f;
    const float MIN_VOLUME = 0.01f;
    public Func<AudioClip> GetNextBgm;
    AudioClip nextClip;
    public float fadeOutThreshold = 0.5f;
    Coroutine fadeRoutine;
    float fadeInVolume;

    public void CrossFadeBgm(AudioClip nextClip)
    {
      this.nextClip = nextClip;
      this.StartFadeOut();
    }

    // Update is called once per frame
    protected override void Update()
    {
      if (this.AudioSource.loop ||
        !this.IsPlaying)  {
        return ;
      }
      this.remainingPlayTime -= Time.deltaTime;
      if (this.remainingPlayTime <= this.fadeOutThreshold) {
        this.StartFadeOut();
      }
    }

    void StartFadeOut()
    {
      this.fadeInVolume = this.AudioSource.volume;
      if (this.fadeRoutine != null) {
        this.StopCoroutine(this.fadeRoutine);
      }
      this.fadeRoutine = this.StartCoroutine(this.FadeOutRoutine());
    }

    void OnFadeOut()
    {
      this.fadeRoutine = null;
      if (this.nextClip != null) {
        this.PlaySound(this.nextClip);
        this.nextClip = null;
      }
      else if (this.GetNextBgm != null) {
        AudioClip nextClip = this.GetNextBgm();
        this.PlaySound(nextClip);
      }
      else {
        this.SetVolume(this.fadeInVolume);
        return ;
      }
      this.fadeRoutine = this.StartCoroutine(this.FadeInRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
      while (this.AudioSource.volume > MIN_VOLUME) {
        this.AudioSource.volume -= FADE_VOLUME_DELTA * Time.deltaTime;
        yield return (null);
      }
      this.OnFadeOut();
    }

    IEnumerator FadeInRoutine()
    {
      while (this.AudioSource.volume < this.fadeInVolume) {
        this.AudioSource.volume += FADE_VOLUME_DELTA * Time.deltaTime;
        yield return (null);
      }
      this.fadeRoutine = null;
    }
  }
}
