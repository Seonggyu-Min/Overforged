using SHG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


namespace MIN
{
    public class UIButtonSoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private AudioClip _hoverSound;
        [SerializeField] private AudioClip _clickSound;


        public void OnPointerClick(PointerEventData eventData)
        {
            SfxController sfx = SingletonAudio.Instance.PlaySfx(_clickSound);
            sfx.SetVolume(0.5f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SfxController sfx = SingletonAudio.Instance.PlaySfx(_hoverSound);
            sfx.SetVolume(0.5f);
        }
    }
}
