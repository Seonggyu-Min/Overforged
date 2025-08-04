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
    [Inject] private IFirebaseManager _firebaseManager;

    [Header("Parent")]
    [SerializeField] private Transform _globalUsersParent;
    [SerializeField] private Transform _friendUsersParent;
    [SerializeField] private Transform _popUpParent;

    [Header("Prefab")]
    [SerializeField] private UserBoxItem _userBoxPrefab;

    [Header("Remove Time")]
    [SerializeField] private int _removalTime = 30000; // 30초

    private bool _isGlobal = true;
    private DatabaseReference _statusRef;
    private Dictionary<string, GameObject> _userUIMap = new();

    private void Awake()
    {
        _statusRef = _firebaseManager.Database.GetReference("onlineStatus");
        _statusRef.ChildAdded += OnUserAdded;
        _statusRef.ChildChanged += OnUserChanged;
        _statusRef.ChildRemoved += OnUserRemoved;
    }

    private void OnEnable()
    {
        _isGlobal = true;
        ChangeList();
    }

    private void OnDisable()
    {
        if (_statusRef != null)
        {
            _statusRef.ChildAdded -= OnUserAdded;
            _statusRef.ChildChanged -= OnUserChanged;
            _statusRef.ChildRemoved -= OnUserRemoved;
        }
    }

    public void OnClickGlobalButton()
    {
        _isGlobal = true;
        ChangeList();
    }

    public void OnClickFriendButton()
    {
        _isGlobal = false;
        ChangeList();
    }

    private void ChangeList()
    {
        _globalUsersParent.gameObject.SetActive(_isGlobal);
        _friendUsersParent.gameObject.SetActive(!_isGlobal);

        foreach (var item in _userUIMap.Values)
        {
            Destroy(item.gameObject);
        }
        _userUIMap.Clear();

        _statusRef.GetValueAsync().ContinueWithOnMainThread(task =>
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

    private void OnUserAdded(object sender, ChildChangedEventArgs e) => UpdateUserStatus(e.Snapshot);
    private void OnUserChanged(object sender, ChildChangedEventArgs e) => UpdateUserStatus(e.Snapshot);
    private void OnUserRemoved(object sender, ChildChangedEventArgs e) => RemoveUserFromUI(e.Snapshot.Key);

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
            AddOrUpdateUserUI(uid, nickname, isGaming);
        }
        else
        {
            RemoveUserFromUI(uid);
        }
    }

    private void AddOrUpdateUserUI(string uid, string nickname, bool isGaming)
    {
        if (_userUIMap.TryGetValue(uid, out var go))
        {
            go.GetComponent<UserBoxItem>().Init(nickname, isGaming, uid, _firebaseManager, _popUpParent);
        }
        else
        {
            var parent = _isGlobal ? _globalUsersParent : _friendUsersParent;

            GameObject ui = Instantiate(_userBoxPrefab.gameObject, parent);
            ui.GetComponent<UserBoxItem>().Init(nickname, isGaming, uid, _firebaseManager, _popUpParent);
            _userUIMap[uid] = ui;
        }
    }

    private void RemoveUserFromUI(string uid)
    {
        if (_userUIMap.TryGetValue(uid, out var go))
        {
            Destroy(go);
            _userUIMap.Remove(uid);
        }
    }
}
