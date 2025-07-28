using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public interface IAudioManager 
    {
        public void PlayBGM();
        public void PlaySFX();
        public void StopBGM();
        public void StopAllSFX();
        public void SetBGMVolume(float volume);
        public void SetSFXVolume(float volume);
        public void GetBGMVolume();
        public void GetSFXVolume();
        public void MuteBGM();
        public void MuteSFX();
    }
}
