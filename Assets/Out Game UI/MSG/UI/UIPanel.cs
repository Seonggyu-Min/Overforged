using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator _animator;

        private void Awake()
        {
            if (_animator == null)
                TryGetComponent<UIAnimator>(out _animator);
        }

        private void OnEnable()
        {
            ShowAnimation();
        }

        public void ShowAnimation()
        {
            if (_animator != null)
            {
                _animator.AnimateIn();
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void HideAnimation()
        {
            if (_animator != null)
            {
                _animator.AnimateOut(() =>
                {
                    gameObject.SetActive(false);
                });
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
