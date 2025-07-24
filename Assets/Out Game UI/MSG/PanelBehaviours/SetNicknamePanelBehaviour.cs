using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class SetNicknamePanelBehaviour : MonoBehaviour
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
                        _outGameUIManager.Hide("Set Nickname Panel", () =>
                        {
                            _outGameUIManager.Clear();
                            _outGameUIManager.Show("Lobby Panel");
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

        #endregion
    }
}
