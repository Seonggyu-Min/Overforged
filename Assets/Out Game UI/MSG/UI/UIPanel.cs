using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MIN
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator _animator;
        [SerializeField] private bool _isRootPanel = false;             // 더 이상 뒤로 갈 수 없는 패널인지 여부 (ex. 로그인 패널, 로비 패널 등) 
        [SerializeField] private bool _hasToLogOutWhenClosed = false;   // 패널이 닫힐 때 로그아웃해야 하는지 여부 (ex. 닉네임 설정 패널, 이메일 인증 대기 패널 등)
        [SerializeField] private bool _isPopUp = false;                 // 팝업 패널인지 여부 (ex. 방 생성 등)

        public bool IsRootPanel => _isRootPanel;
        public bool HasToLogOutWhenClosed => _hasToLogOutWhenClosed;
        public bool IsPopUp => _isPopUp;


        private void Awake()
        {
            if (_animator == null)
            {
                TryGetComponent<UIAnimator>(out _animator);
            }
        }

        private void OnEnable() => ShowAnimation();


        public void ShowAnimation()
        {
            if (_animator != null)
            {
                StartCoroutine(_animator.AnimateIn());
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        public void HideAnimation(Action onComplete = null)
        {
            if (_animator != null)
            {
                StartCoroutine(_animator.AnimateOut(() =>
                {
                    gameObject.SetActive(false);
                    onComplete?.Invoke();
                }));
            }
            else // 애니메이션이 없는 경우 바로 비활성화
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }
        }
    }
}
