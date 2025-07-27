using Firebase.Auth;
using Firebase.Extensions;
using ModestTree;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class LogInPanelBehaviour : MonoBehaviourPun
    {
        #region Fields And Properties

        [Inject] private IFirebaseManager _firebaseManager;
        [Inject] private IOutGameUIManager _outGameUIManager;

        [Header("로그인 패널 요소 등록")]
        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private TMP_InputField _passwordInputField;

        [SerializeField] private Button _findButton;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _signUpButton;

        #endregion


        #region Public Methods

        public void OnClickFindButton() => Find();

        public void OnClickLoginButton() => LogIn();

        public void OnClickSignUpButton() => SignUp();

        #endregion


        #region Private Methods

        private void Find()
        {
            _outGameUIManager.Hide("Log In Panel", () =>
            {
                _outGameUIManager.Show("Find Panel");
            });
        }

        private void LogIn()
        {
            _firebaseManager.Auth.SignInWithEmailAndPasswordAsync(_idInputField.text, _passwordInputField.text)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("로그인이 취소됨");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError($"로그인에 실패함. 이유 : {task.Exception}");
                        return;
                    }

                    FirebaseUser user = task.Result.User;

                    if (user.IsEmailVerified == true)
                    {
                        // 1-1. 아직 닉네임 설정을 하지 않은 경우
                        if (user.DisplayName == "")
                        {
                            _outGameUIManager.Hide("Log In Panel", () =>
                            {
                                _outGameUIManager.Show("Set Nickname Panel");
                            });

                        }
                        // 1-2. 닉네임 설정도 완료한 경우
                        else
                        {
                            if (!PhotonNetwork.IsConnected)
                            {
                                Debug.Log("Photon 네트워크에 연결되지 않았습니다. 연결을 시도합니다.");
                                PhotonNetwork.ConnectUsingSettings();
                            }                            

                            Debug.Log("로그인 성공, 로딩화면으로 이동합니다.");
                            _outGameUIManager.Hide("Log In Panel", () =>
                            {
                                _outGameUIManager.Show("Loading Panel");
                            });
                        }
                    }
                    else
                    {
                        // 2. 이메일 인증을 아직 하지 않은 경우
                        _outGameUIManager.Hide("Log In Panel", () =>
                        {
                            _outGameUIManager.Show("Wait For Email Panel");
                        });
                    }
                });

        }

        private void SignUp()
        {
            _outGameUIManager.Hide("Log In Panel", () =>
            {
                _outGameUIManager.Show("Sign Up Panel");
            });
        }

        #endregion
    }
}
