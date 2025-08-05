using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class AddFriendPopUp : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;

        [SerializeField] private OtherUserInfoPopUp _otherUserInfo;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _info2Text;

        private void OnEnable()
        {
            _infoText.text = $"Will You Add {_otherUserInfo.NickName} To Your Friend?";
            _info2Text.text = string.Empty;
        }


        public void OnClickYesButton()
        {
            var myUid = _firebaseManager.Auth.CurrentUser.UserId;
            var targetUid = _otherUserInfo.UID;
            var db = _firebaseManager.Database;

            var userRef = db.GetReference("users");
            var requestRef = db.GetReference("friend_requests");

            // 1. 존재 여부 확인
            userRef.Child(targetUid).GetValueAsync().ContinueWithOnMainThread(userTask =>
            {
                if (userTask.IsCanceled || !userTask.IsCompletedSuccessfully || userTask.Result == null || !userTask.Result.Exists)
                {
                    Debug.LogWarning($"유저 확인 실패 또는 존재하지 않음: {userTask.Exception}");
                    return;
                }

                // 2. 이미 친구인지 확인
                userRef.Child(myUid).Child("friends").Child(targetUid)
                .GetValueAsync().ContinueWithOnMainThread(friendTask =>
                {
                    if (friendTask.IsCanceled || !friendTask.IsCompletedSuccessfully || friendTask.Result == null)
                    {
                        Debug.LogWarning($"친구 확인 실패: {friendTask.Exception}");
                        return;
                    }

                    if (friendTask.Result.Exists)
                    {
                        _info2Text.text = "이미 친구입니다.";
                        return;
                    }

                    // 3. 이미 요청을 보냈는지 확인
                    requestRef.Child(targetUid).Child(myUid)
                    .GetValueAsync().ContinueWithOnMainThread(requestTask =>
                    {
                        if (requestTask.IsCanceled || !requestTask.IsCompletedSuccessfully || requestTask.Result == null)
                        {
                            Debug.LogWarning($"요청 확인 실패: {requestTask.Exception}");
                            return;
                        }

                        if (requestTask.Result.Exists)
                        {
                            _info2Text.text = "이미 친구 요청을 보낸 상태입니다.";
                            return;
                        }

                        // 4. 요청 등록
                        requestRef.Child(targetUid).Child(myUid).SetValueAsync(true)
                        .ContinueWithOnMainThread(sendTask =>
                        {
                            if (sendTask.IsCanceled || !sendTask.IsCompletedSuccessfully)
                            {
                                Debug.LogWarning($"요청 전송 실패: {sendTask.Exception}");
                                return;
                            }
                        });
                    });
                });
            });

            _info2Text.text = $"{_otherUserInfo.NickName}님 에게 친구 요청을 보냈습니다. 곧 창이 닫힙니다.";
            StartCoroutine(CloseRoutine());
        }

        public void OnClickNoButton()
        {
            gameObject.SetActive(false);
        }


        private IEnumerator CloseRoutine()
        {
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }
    }
}
