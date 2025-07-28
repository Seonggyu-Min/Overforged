using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class SetNicknamePanelBehaviour : MonoBehaviourPun
    {
        #region Fields And Properties
        
        [Inject] private IFirebaseManager _firebaseManager;
        [Inject] private IOutGameUIManager _outGameUIManager;

        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private TMP_Text _infoText;

        [SerializeField] private Button _checkButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;

        #endregion


        #region Unity Methods

        private void OnEnable() => Init();

        #endregion


        #region Public Methods

        public void OnClickCheckButton() => Check();

        public void OnClickConfirm() => Confirm();

        public void OnClickCancel() => _outGameUIManager.CloseTopPanel();

        #endregion


        #region Private Methods

        private void Init()
        {
            _confirmButton.interactable = false;
            _infoText.text = "닉네임을 입력하고 중복 확인을 해주세요.";

            ClearText();
        }

        private void Confirm()
        {
            FirebaseUser user = _firebaseManager.Auth.CurrentUser;
            string userId = user.UserId;

            UserProfile profile = new UserProfile { DisplayName = _nicknameInputField.text };

            user.UpdateUserProfileAsync(profile)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        _infoText.text = "닉네임 업데이트가 취소되었습니다.";
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        _infoText.text = $"닉네임 업데이트 중 오류 발생: {task.Exception}";
                        return;
                    }

                    // 유저 정보 저장
                    DatabaseReference db = _firebaseManager.Database.RootReference;
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        [$"users/{userId}/nickname"] = _nicknameInputField.text,
                        [$"nicknames/{_nicknameInputField.text}"] = userId
                    };

                    db.UpdateChildrenAsync(updates).ContinueWithOnMainThread(setTask =>
                    {
                        if (setTask.IsCanceled || setTask.IsFaulted)
                        {
                            _infoText.text = "닉네임 저장 중 오류가 발생했습니다.";
                            return;
                        }

                        _infoText.text = "닉네임이 성공적으로 설정되었습니다.";

                        SetNickName();

                        PhotonNetwork.ConnectUsingSettings();

                        _outGameUIManager.Hide("Set Nickname Panel", () =>
                        {
                            _outGameUIManager.Show("Loading Panel");
                        });
                    });
                });

        }

        private void Check()
        {
            string nickname = _nicknameInputField.text;

            _firebaseManager.Database
                .GetReference("nicknames")
                .Child(nickname)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        _infoText.text = "중복 확인 중 오류가 발생했습니다.";
                        return;
                    }

                    if (task.Result.Exists)
                    {
                        _infoText.text = "이미 사용 중인 닉네임입니다.";
                    }
                    else
                    {
                        _infoText.text = "사용 가능한 닉네임입니다.";
                        _confirmButton.interactable = true;
                    }
                });
        }

        private void SetNickName()
        {
            FirebaseUser user = _firebaseManager.Auth.CurrentUser;

            if (user != null)
            {
                string uid = user.UserId;

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
                    });
            }
            else
            {
                Debug.LogWarning("Firebase 사용자 정보가 없습니다. Nickname 설정을 건너뜁니다.");
                return;
            }
        }

        private void ClearText()
        {
            _nicknameInputField.text = string.Empty;
        }

        #endregion
    }
}
