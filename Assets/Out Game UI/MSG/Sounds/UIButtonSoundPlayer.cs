using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


namespace MIN
{
    public class UIButtonSoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [Inject] IAudioLibrary _audioLibrary;

        [SerializeField] private AudioClip _hoverSound;
        [SerializeField] private AudioClip _clickSound;


        public void OnPointerClick(PointerEventData eventData)
        {
            _audioLibrary.PlaySfx(_clickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioLibrary.PlaySfx(_hoverSound);
        }
    }
}
