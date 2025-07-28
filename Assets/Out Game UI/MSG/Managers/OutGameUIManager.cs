using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class OutGameUIManager : MonoBehaviour, IOutGameUIManager
    {
        #region Fields And Properties

        [Inject] private IFirebaseManager _firebaseManager;

        private Dictionary<string, UIPanel> _panels = new();
        private Stack<UIPanel> _panelStack = new();

        #endregion


        #region Unity Methods

        private void Start()
        {
            // 로그인 안 되어있으면 로그인 화면
            if (_firebaseManager.Auth.CurrentUser == null)
            {
                ShowAsFirst("Log In Panel");
            }
            // 게임 중이었다가 돌아온 경우
            else if (PhotonNetwork.InRoom)
            {
                _panelStack.Push(_panels["Lobby Panel"]); // 로비패널 스택에 넣어 뒤로갈 수 있도록 함
                ShowAsFirst("Room Panel");
            }
            // 예외 상황
            else
            {
                Debug.LogWarning("로그인되어 있지만 방에 들어가있지 않습니다.");
                ShowAsFirst("Lobby Panel");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTopPanel();
            }
        }

        #endregion


        #region Public Methods

        public void RegisterPanel(string key, UIPanel panel)
        {
            if (!_panels.ContainsKey(key))
            {
                _panels.Add(key, panel);
            }
            else
            {
                Debug.LogWarning($"패널 키{key}가 이미 등록되어있습니다. 등록에 실패했습니다.");
            }
        }

        public void Show(string key)
        {
            if (_panels.TryGetValue(key, out UIPanel panel))
            {
                if (_panelStack.Contains(panel)) return;    // 이미 스택에 있는 경우 return

                if (panel.IsRootPanel)
                {
                    _panelStack.Clear(); // 루트 패널을 보여줄 때는 스택 비우기
                }

                panel.gameObject.SetActive(true);
                _panelStack.Push(panel);
            }
            else
            {
                Debug.LogWarning($"패널 키{key}가 등록되어 있지 않습니다.");
            }
        }

        public void ShowAsFirst(string key)
        {
            if (_panels.TryGetValue(key, out UIPanel panel))
            {
                _panelStack.Clear();
                panel.gameObject.SetActive(true);
                _panelStack.Push(panel);
            }
        }

        public void Hide(string key, Action onComplete = null)
        {
            if (_panels.TryGetValue(key, out UIPanel panel))
            {
                if (!_panelStack.Contains(panel)) return;   // 스택에 없는 경우 return
                panel.HideAnimation(onComplete);
            }
        }

        public void CloseTopPanel()
        {
            if (_panelStack.Count > 1)
            {
                UIPanel topPanel = _panelStack.Peek();

                if (topPanel.IsRootPanel)
                {
                    Debug.Log("뒤로 갈 수 없는 패널입니다");
                    return;
                }

                if (topPanel.IsPopUp) // 팝업 패널은 그냥 닫기
                {
                    topPanel.HideAnimation();
                    _panelStack.Pop();
                    return;
                }

                if (topPanel.HasToLeaveRoomWhenClosed && PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                }

                _panelStack.Pop();
                UIPanel previousPanel = _panelStack.Peek();

                if (topPanel.HasToLogOutWhenClosed)
                {
                    _firebaseManager.Auth.SignOut();
                }

                if (topPanel != null)
                {
                    topPanel.HideAnimation(() =>
                    {
                        if (previousPanel != null)
                        {
                            previousPanel.gameObject.SetActive(true);
                            previousPanel.ShowAnimation();
                        }
                    });
                }
            }
            else
            {
                //Debug.Log($"현재 패널 스택 카운트: {_panelStack.Count}, 더 이상 닫을 수 없습니다.");
            }
        }

        public void Clear()
        {
            _panelStack.Clear();
        }

        #endregion
    }
}
