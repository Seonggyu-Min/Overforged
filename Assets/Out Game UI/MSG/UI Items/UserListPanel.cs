using Firebase.Database;
using Firebase.Extensions;
using MIN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UserListPanel : MonoBehaviour
{
    #region Fields and Properties

    [Inject] private IFirebaseManager _firebaseManager;
    [Inject] private DiContainer _diContainer;

    [Header("Parent")]
    [SerializeField] private GameObject _globalUsersObj;
    [SerializeField] private Transform _globalUsersParent;
    [SerializeField] private GameObject _friendUsersObj;
    [SerializeField] private Transform _friendUsersParent;
    [SerializeField] private Transform _popUpParent;

    [Header("Prefab")]
    [SerializeField] private UserBoxItem _userBoxPrefab;

    [Header("Time Settings")]
    [SerializeField] private int _removalTime = 30000; // (_removalTime / 1000) 초 동안 접속 시간이 없으면 오프라인으로 간주
    [SerializeField, Min(5)] private int _checkInterval = 10; // _checkInterval 초 마다 접속자 리스트 확인

    private DatabaseReference _globalStatusRef;
    private string _myUid; // 현재 로그인한 사용자 UID
    private bool _isGlobal = true;
    private Coroutine _statusCheckCO;

    private Dictionary<string, GameObject> _globalUIMap = new();
    private Dictionary<string, GameObject> _friendsUIMap = new();
    private HashSet<string> _friendsUidMap = new();

    #endregion


    #region Unity Methods

    private void Awake()
    {
        _globalStatusRef = _firebaseManager.Database.GetReference("onlineStatus");
        _globalStatusRef.ChildAdded += OnUserAdded;
        _globalStatusRef.ChildChanged += OnUserChanged;
        _globalStatusRef.ChildRemoved += OnUserRemoved;
    }

    private void OnEnable()
    {
        _isGlobal = true;
        RefreshList();
        _statusCheckCO = StartCoroutine(StatusCheckRoutine());
    }

    private void OnDisable()
    {
        if (_globalStatusRef != null)
        {
            _globalStatusRef.ChildAdded -= OnUserAdded;
            _globalStatusRef.ChildChanged -= OnUserChanged;
            _globalStatusRef.ChildRemoved -= OnUserRemoved;
        }

        if (_statusCheckCO != null)
        {
            StopCoroutine(_statusCheckCO);
            _statusCheckCO = null;
        }
    }

    #endregion


    #region Public Methods

    public void OnClickGlobalButton()
    {
        if (_isGlobal) return;

        _isGlobal = true;
        RefreshList();

        Debug.Log($"_isGlobal: {_isGlobal}");
    }

    public void OnClickFriendButton()
    {
        if (!_isGlobal) return;

        _isGlobal = false;
        RefreshList();

        Debug.Log($"_isGlobal: {_isGlobal}");
    }

    #endregion


    #region Private Methods
    private void OnUserAdded(object sender, ChildChangedEventArgs e) => UpdateUserStatus(e.Snapshot);
    private void OnUserChanged(object sender, ChildChangedEventArgs e) => UpdateUserStatus(e.Snapshot);
    private void OnUserRemoved(object sender, ChildChangedEventArgs e) => RemoveUserBox(e.Snapshot.Key);

    private void RefreshList()
    {
        _globalUsersObj.SetActive(_isGlobal);
        _friendUsersObj.SetActive(!_isGlobal);

        foreach (var ui in _globalUIMap)
        {
            Destroy(ui.Value);
        }
        foreach (var ui in _friendsUIMap)
        {
            Destroy(ui.Value);
        }

        _globalUIMap.Clear();
        _friendsUIMap.Clear();

        if (_isGlobal)
        {
            _globalStatusRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    foreach (var userSnapshot in task.Result.Children)
                    {
                        UpdateUserStatus(userSnapshot);
                    }
                }
                else
                {
                    Debug.LogWarning("onlineStatus가 정상적으로 로드되지 않았습니다.");
                }
            });
        }
        else
        {
            RefreshFriendsList();
        }
    }

    private void UpdateUserStatus(DataSnapshot snapshot)
    {
        string uid = snapshot.Key;
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        long lastSeen = long.TryParse(snapshot.Child("lastSeen")?.Value?.ToString(), out var l) ? l : 0;
        bool isGaming = bool.TryParse(snapshot.Child("isGaming")?.Value?.ToString(), out var g) && g;
        string nickname = snapshot.Child("nickname")?.Value?.ToString() ?? "Unknown";
        bool isOnline = now - lastSeen < _removalTime;

        if (isOnline)
        {
            ShowUserBox(uid, nickname, isGaming, isOnline);
        }
        else
        {
            RemoveUserBox(uid);
        }
    }

    private void ShowUserBox(string uid, string nickname, bool isGaming, bool isOnline)
    {
        if (!_isGlobal && !_friendsUidMap.Contains(uid))
        {
            Debug.Log($"친구가 아니라서 필터링됨: {uid}");
            return;
        }

        var uiMap = _isGlobal ? _globalUIMap : _friendsUIMap;
        var parent = _isGlobal ? _globalUsersParent : _friendUsersParent;

        if (uiMap.TryGetValue(uid, out var go))
        {
            go.GetComponent<UserBoxItem>().Init(nickname, isGaming, uid, _popUpParent, isOnline);
        }
        else
        {
            GameObject ui = Instantiate(_userBoxPrefab.gameObject, parent);
            _diContainer.InjectGameObject(ui);
            ui.GetComponent<UserBoxItem>().Init(nickname, isGaming, uid, _popUpParent, isOnline);
            uiMap[uid] = ui;
        }
    }

    private void RemoveUserBox(string uid)
    {
        // 친구는 오프라인에서도 표기
        if (_globalUIMap.TryGetValue(uid, out var go))
        {
            Destroy(go);
            _globalUIMap.Remove(uid);
        }
    }

    private void RefreshFriendsList()
    {
        _myUid = _firebaseManager.Auth.CurrentUser.UserId;

        DatabaseReference userFriendsRef = _firebaseManager.Database.GetReference($"users/{_myUid}/friends");

        userFriendsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully && task.Result.Exists)
            {
                _friendsUidMap.Clear();
                foreach (var friend in task.Result.Children)
                {
                    if (friend.Key == _myUid) continue;
                    _friendsUidMap.Add(friend.Key);
                }

                if (!_isGlobal)
                {
                    UpdateFriendUI(); // 친구 목록만 업데이트
                }
            }
        });
    }

    private void UpdateFriendUI()
    {
        foreach (var uid in _friendsUidMap)
        {
            DatabaseReference friendUserRef = _firebaseManager.Database.GetReference($"users").Child(uid);

            friendUserRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"친구 정보 조회 실패: {task.Exception}");
                    return;
                }

                if (!task.Result.Exists)
                {
                    Debug.Log("친구가 없어 return");
                    return;
                }

                string nickname = task.Result.Child("nickname")?.Value?.ToString() ?? "Unknown";

                // 친구의 onlineStatus 조회
                DatabaseReference friendStatusRef = _firebaseManager.Database.GetReference($"onlineStatus").Child(uid);

                friendStatusRef.GetValueAsync().ContinueWithOnMainThread(statusTask =>
                {
                    if (statusTask.IsFaulted || statusTask.IsCanceled)
                    {
                        Debug.LogWarning($"친구 상태 조회 실패: {statusTask.Exception}");
                        return;
                    }

                    bool isGaming = bool.TryParse(statusTask.Result.Child("isGaming")?.Value?.ToString(), out var g) && g;
                    long lastSeen = long.TryParse(statusTask.Result.Child("lastSeen")?.Value?.ToString(), out var l) ? l : 0;
                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    bool isOnline = now - lastSeen < _removalTime;

                    ShowUserBox(uid, nickname, isGaming, isOnline);
                });
            });
        }
    }

    private IEnumerator StatusCheckRoutine()
    {
        WaitForSeconds wait = new(_checkInterval);

        while (true)
        {
            yield return wait;

            if (_isGlobal)
            {
                _globalStatusRef.GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogWarning($"글로벌 유저 상태 조회 실패: {task.Exception}");
                        return;
                    }

                    if (!task.Result.Exists)
                    {
                        Debug.Log("글로벌 접속 유저 없음");
                        return;
                    }

                    foreach (var userSnapshot in task.Result.Children)
                    {
                        UpdateUserStatus(userSnapshot);
                    }
                });
            }
            else
            {
                // 친구 상태 조회
                foreach (var uid in _friendsUidMap)
                {
                    var userRef = _firebaseManager.Database.GetReference($"users").Child(uid);
                    var statusRef = _firebaseManager.Database.GetReference($"onlineStatus").Child(uid);

                    // 친구 닉네임
                    userRef.GetValueAsync().ContinueWithOnMainThread(userTask =>
                    {
                        if (userTask.IsFaulted || userTask.IsCanceled)
                        {
                            Debug.LogWarning($"조회 실패: {userTask.Exception}");
                            return;
                        }

                        if (!userTask.Result.Exists)
                        {
                            Debug.Log("친구가 없어 return");
                            return;
                        }

                        string nickname = userTask.Result.Child("nickname")?.Value?.ToString() ?? "Unknown";

                        // 친구 상태
                        statusRef.GetValueAsync().ContinueWithOnMainThread(statusTask =>
                        {
                            if (statusTask.IsFaulted || statusTask.IsCanceled)
                            {
                                Debug.LogWarning($"상태 조회 실패 {statusTask.Exception}");
                                return;
                            }

                            bool isGaming = bool.TryParse(statusTask.Result.Child("isGaming")?.Value?.ToString(), out var g) && g;
                            long lastSeen = long.TryParse(statusTask.Result.Child("lastSeen")?.Value?.ToString(), out var l) ? l : 0;
                            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            bool isOnline = now - lastSeen < _removalTime;

                            ShowUserBox(uid, nickname, isGaming, isOnline);
                        });
                    });
                }
            }
        }
    }

    #endregion
}
