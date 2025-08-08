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
            SingletonAudio.Instance.PlaySfx(_clickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SingletonAudio.Instance.PlaySfx(_hoverSound);
        }
    }
}
