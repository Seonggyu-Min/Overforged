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
    public class FriendAcceptPanel : MonoBehaviour
    {
        #region Fields and Properties

        [Inject] private IFirebaseManager _firebaseManager;

        [Header("UI Components")]
        [SerializeField] private GameObject _popupPanel;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _rejectButton;

        private string _myUid;
        private DatabaseReference _requestRef;
        private DatabaseReference _userRef;
        private bool _isWaitingForInput = false;

        #endregion


        #region Unity Methods

        private void OnEnable()
        {
            _myUid = _firebaseManager.Auth.CurrentUser.UserId;
            _requestRef = _firebaseManager.Database.GetReference("friend_requests").Child(_myUid);
            _userRef = _firebaseManager.Database.GetReference("users");

            _requestRef.ChildAdded += OnRequestAdded;

            _popupPanel.SetActive(false);
            StartCoroutine(ProcessInitialRequests());

        }

        private void OnDisable()
        {
            if (_requestRef != null)
            {
                _requestRef.ChildAdded -= OnRequestAdded;
            }
        }

        #endregion


        #region Private Methods

        private IEnumerator ProcessInitialRequests()
        {
            var requestTask = _requestRef.GetValueAsync();
            yield return new WaitUntil(() => requestTask.IsCompleted);

            if (!requestTask.IsCompletedSuccessfully || requestTask.IsFaulted || requestTask.IsCanceled)
            {
                Debug.Log($"요청 실패: {requestTask.Exception}");
                yield break;
            }

            if (!requestTask.Result.Exists)
            {
                Debug.Log("요청이 없어 종료");
                yield break;
            }

            foreach (var child in requestTask.Result.Children)
            {
                string requesterUid = child.Key;
                yield return StartCoroutine(ShowPopupForRequest(requesterUid));
            }
        }

        private void OnRequestAdded(object sender, ChildChangedEventArgs args)
        {
            string requesterUid = args.Snapshot.Key;
            StartCoroutine(ShowPopupForRequest(requesterUid));
        }

        private IEnumerator ShowPopupForRequest(string requesterUid)
        {
            _isWaitingForInput = true;

            // 닉네임 불러오기
            var nicknameRef = _firebaseManager.Database.GetReference("users")
                .Child(requesterUid)
                .Child("nickname");

            var nicknameTask = nicknameRef.GetValueAsync();
            yield return new WaitUntil(() => nicknameTask.IsCompleted);

            string requesterNickname;

            if (!nicknameTask.IsCompletedSuccessfully || nicknameTask.IsCanceled || nicknameTask.IsFaulted)
            {
                Debug.LogWarning($"닉네임을 가져오지 못했습니다: {nicknameTask.Exception}");
                yield break;
            }
            requesterNickname = nicknameTask.Result.Value.ToString();

            // UI 설정
            _infoText.text = $"{requesterNickname}님이 친구 요청을 보냈습니다.";
            _popupPanel.SetActive(true);

            // 버튼 초기화
            _acceptButton.onClick.RemoveAllListeners();
            _rejectButton.onClick.RemoveAllListeners();

            _acceptButton.onClick.AddListener(() =>
            {
                AcceptRequest(requesterUid);
                ClosePopup();
            });

            _rejectButton.onClick.AddListener(() =>
            {
                RejectRequest(requesterUid);
                ClosePopup();
            });

            // 버튼 대기
            yield return new WaitUntil(() => !_isWaitingForInput);
        }

        private void ClosePopup()
        {
            _popupPanel.SetActive(false);
            _isWaitingForInput = false;
        }

        private void AcceptRequest(string requesterUid)
        {
            _userRef.Child(_myUid).Child("friends").Child(requesterUid).SetValueAsync(true);
            _userRef.Child(requesterUid).Child("friends").Child(_myUid).SetValueAsync(true);
            _requestRef.Child(requesterUid).RemoveValueAsync(); // 요청 목록에서 제거
        }

        private void RejectRequest(string requesterUid)
        {
            _requestRef.Child(requesterUid).RemoveValueAsync(); // 요청 목록에서 제거
        }

        #endregion
    }
}
