using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserListPanel : MonoBehaviour
{
    [SerializeField] private GameObject _globalUsers;
    [SerializeField] private GameObject _friendUsers;
    private bool _isGlobal = true;

    private void OnEnable()
    {
        _isGlobal = true;
        ChangeList();
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
        if (_isGlobal)
        {
            _globalUsers.SetActive(true);
            _friendUsers.SetActive(false);
        }
        else
        {
            _globalUsers.SetActive(false);
            _friendUsers.SetActive(true);
        }
    }
}
