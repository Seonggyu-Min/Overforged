using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class FindPanelBehaviour : MonoBehaviour
    {
        #region Fields And Properties

        [Inject] private IFirebaseManager _firebaseManager;
        [Inject] private IOutGameUIManager _outGameUIManager;

        [SerializeField] private TMP_InputField _emailInput;

        [SerializeField] private PopupUIBehaviour _popup;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Button _backButton;

        #endregion


        #region Unity Methods

        private void OnEnable() => Init();

        #endregion


        #region Public Methods

        public void OnClickSendButton() => Send();

        public void OnClickBackButton() => _outGameUIManager.CloseTopPanel();

        #endregion


        #region Private Methods

        private void Init()
        {
            ClearText();
            _popup.ShowInfo("비밀번호를 재설정할 이메일을 입력하세요.");
        }

        private void Send()
        {
            string email = _emailInput.text;

            if (string.IsNullOrEmpty(email))
            {
                _popup.ShowError("이메일을 입력해주세요.");
                return;
            }

            _firebaseManager.Auth.SendPasswordResetEmailAsync(email)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        _popup.ShowError("인증 이메일 전송이 취소되었습니다. 다시 시도해주세요.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        _popup.ShowError($"인증 이메일 전송 중 오류 발생: {task.Exception}");
                        return;
                    }

                    _popup.ShowInfo("비밀번호 재설정 이메일이 발송되었습니다.");
                });
        }

        private void ClearText()
        {
            _emailInput.text = string.Empty;
        }

        #endregion
    }
}

