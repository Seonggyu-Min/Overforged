using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class AudioManager : IAudioManager, IInitializable
    {
        private const float MIN_VOLUME = 0.0f;
        private const float MAX_VOLUME = 1.0f;

        private float _currentBGMVolume = 0.5f;
        private float _currentSFXVolume = 0.5f;


        #region Zenject Methods

        public void Initialize()
        {
            CreateBGM();
            CreateSFXPool();
        }

        #endregion


        #region Public Methods

        public void PlayBGM()
        {

        }

        public void PlaySFX()
        {

        }

        public void StopBGM()
        {

        }

        public void StopAllSFX()
        {

        }

        public void SetBGMVolume(float volume)
        {

        }

        public void SetSFXVolume(float volume)
        {

        }

        public void GetBGMVolume()
        {

        }

        public void GetSFXVolume()
        {

        }

        public void MuteBGM()
        {

        }

        public void MuteSFX()
        {

        }

        #endregion


        #region Private Methods

        private void CreateBGM()
        {

        }

        private void CreateSFXPool()
        {

        }

        #endregion
    }
}
