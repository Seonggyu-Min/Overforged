using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class SignUpPanelBehaviour : MonoBehaviour
    {
        #region Fields And Properties

        [Inject] private IFirebaseManager _firebaseManager;
        [Inject] private IOutGameUIManager _outGameUIManager;

        [Header("회원가입 패널 요소 등록")]

        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_InputField _passwordConfirmInputField;

        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;

        [SerializeField] private TMP_Text _infoText;

        [Header("안내 메시지 코루틴 설정")]
        [SerializeField] private float _infoTextDisplayTime = 3f;
        private Coroutine _infoTextCo;

        #endregion




        #region Public Methods

        public void OnClickConfirmButton()
        {
            SignUp();
        }

        public void OnClickCancelButton()
        {
            _outGameUIManager.CloseTopPanel();
        }

        #endregion


        #region Private Methods

        private void SignUp()
        {
            if (_passwordInputField.text != _passwordConfirmInputField.text)
            {
                _infoText.text = "비밀번호가 일치하지 않습니다.";
                StartInfoTextCoroutine();
                return;
            }

            _firebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(_idInputField.text, _passwordInputField.text)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        _infoText.text = "회원가입이 취소되었습니다.";
                        StartInfoTextCoroutine();
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        _infoText.text = "회원가입 중 오류가 발생했습니다.";
                        StartInfoTextCoroutine();
                        return;
                    }

                    _infoText.text = "회원가입이 완료되었습니다.";
                    StartInfoTextCoroutine();

                    _outGameUIManager.Hide("Sign Up Panel");
                    _outGameUIManager.Show("Log In Panel");
                });
        }

        private IEnumerator ShowInfoTextRoutine()
        {
            _infoText.gameObject.SetActive(true);
            yield return new WaitForSeconds(_infoTextDisplayTime);
            _infoText.gameObject.SetActive(false);
        }

        private void StartInfoTextCoroutine()
        {
            if (_infoTextCo != null)
            {
                StopCoroutine(_infoTextCo);
            }

            _infoTextCo = StartCoroutine(ShowInfoTextRoutine());
        }

        #endregion
    }
}
