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
            // 혹시 _firebaseManager.Auth.CurrentUser == null 이 아닌 상태로 시작할 수 있나?
            if (_firebaseManager.Auth.CurrentUser == null)
            {
                _panelStack.Clear();
                ShowAsFirst("Log In Panel");
            }
            // 게임 중이었다가 돌아온 경우
            else if (PhotonNetwork.InRoom)
            {
                _panelStack.Clear();
                _panelStack.Push(_panels["Lobby Panel"]); // 로비패널 스택에 넣어 뒤로갈 수 있도록 함
                ShowAsFirst("Room Panel");
            }
            // 예외 상황
            // 로그인 상황이 로컬 캐시에 남아있고, GameQuitManager의 TrySignOut이 아직 반영되지 않은 경우 해당 예외 상황으로 들어갈 것 같음
            // 근데 최소 로그인 이전까지는 SignOut처리가 될 것 같아서 큰 문제는 없어보이는데, 아래의 디버그 로그가 나왔을 때 추가적인 버그가 있는지 확인해야될 듯
            // 또한 방이 터질 수 있다면 다시 로그인 화면으로 돌아오게될 텐데, 만약 그런 현상이 보인다면 Show하는 패널의 조건 분기 처리 해야될 듯
            else
            {
                Debug.LogWarning("로그인되어 있지만 방에 들어가있지 않습니다.");
                _panelStack.Clear();
                ShowAsFirst("Log In Panel");
            }

            // 디버그용
            //foreach (var panel in _panels)
            //{
            //    Debug.Log($"등록된 패널: {panel.Key}");
            //}

            //UIPanel[] panelArray = _panelStack.ToArray();
            //Array.Reverse(panelArray); 

            //Debug.Log("현재 패널 스택 상태 (Top → Bottom):");
            //foreach (var panel in panelArray)
            //{
            //    Debug.Log($"- {panel.name}");
            //}
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

        public UIPanel GetPanel(string panelName)
        {
            if (_panels.TryGetValue(panelName, out UIPanel panel))
            {
                return panel;
            }
            else
            {
                Debug.LogError($"패널 이름 '{panelName}'이 등록되어 있지 않습니다.");
                return null;
            }
        }

        #endregion
    }
}
