using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using ModestTree;
using Photon.Pun;
using Photon.Realtime;
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


        #region Unity Methods

        private void OnEnable() => ClearText();

        #endregion


        #region Public Methods

        public void OnClickFindButton() => Find();

        public void OnClickLoginButton() => LogIn();

        public void OnClickSignUpButton() => SignUp();

        public void OnClickExitButton() => Exit();

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
            if (_firebaseManager.Auth.CurrentUser != null)
            {
                _firebaseManager.Auth.SignOut();
            }

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

                    // 이메일 인증을 아직 하지 않은 경우
                    if (user.IsEmailVerified == false)
                    {
                        _outGameUIManager.Hide("Log In Panel", () =>
                        {
                            _outGameUIManager.Show("Wait For Email Panel");
                        });

                        return;
                    }

                    _firebaseManager.Database.GetReference("users")
                    .Child(user.UserId)
                    .Child("nickname")
                    .GetValueAsync()
                    .ContinueWithOnMainThread(nickTask =>
                    {
                        if (nickTask.IsFaulted || nickTask.IsCanceled)
                        {
                            Debug.LogError("닉네임 확인 실패");
                            return;
                        }
                        
                        bool hasNickname = nickTask.Result.Exists && !string.IsNullOrEmpty(nickTask.Result.Value?.ToString());

                        // 닉네임이 설정되지 않았을 경우
                        if (hasNickname == false)
                        {
                            _outGameUIManager.Hide("Log In Panel", () =>
                            {
                                _outGameUIManager.Show("Set Nickname Panel");
                            });
                        }
                        // 닉네임이 설정 되었을 경우
                        else
                        {
                            SetNickName(() =>
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
                            });
                        }
                    });
                });
        }

        private void SignUp()
        {
            _outGameUIManager.Hide("Log In Panel", () =>
            {
                _outGameUIManager.Show("Sign Up Panel");
            });
        }

        private void SetNickName(System.Action onComplete)
        {
            FirebaseUser user = _firebaseManager.Auth.CurrentUser;

            if (user != null)
            {
                string uid = user.UserId;

                PhotonNetwork.AuthValues = new AuthenticationValues(user.UserId);
                Debug.Log($"PhotonNetwork.AuthValues 설정: UserId: {user.UserId}");
                
                if (PhotonNetwork.IsConnected)
                {
                    Debug.Log("AuthValues 설정 전에 Photon 네트워크에 이미 연결되어 있습니다.");
                }
                else
                {
                    Debug.Log("AuthValues 설정 전에 Photon 네트워크에 연결되지 않았습니다.");
                }

                _firebaseManager.Database
                    .GetReference("users")
                    .Child(uid)
                    .GetValueAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("사용자 정보를 가져오는 작업이 취소됨");
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            Debug.LogError($"사용자 정보를 가져오는 데 실패함. 이유: {task.Exception}");
                            return;
                        }

                        DataSnapshot snapshot = task.Result;
                        if (snapshot.Exists)
                        {
                            string nickname = snapshot.Child("nickname").Value.ToString();
                            PhotonNetwork.NickName = nickname;
                            Debug.Log($"닉네임 설정 완료: {nickname}");
                        }
                        else
                        {
                            Debug.LogWarning("사용자 정보가 존재하지 않습니다. 기본 닉네임을 설정합니다.");
                            PhotonNetwork.NickName = "Guest";
                        }

                        onComplete?.Invoke();
                    });
            }
            else
            {
                Debug.LogWarning("Firebase 사용자 정보가 없습니다. Nickname 설정을 건너뜁니다.");
                PhotonNetwork.NickName = "Guest";
                onComplete?.Invoke();
            }
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ClearText()
        {
            _idInputField.text = string.Empty;
            _passwordInputField.text = string.Empty;
        }



#endregion
    }
}
