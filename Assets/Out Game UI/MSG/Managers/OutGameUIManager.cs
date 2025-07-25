using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class OutGameUIManager : MonoBehaviour, IOutGameUIManager
    {
        #region Fields And Properties

        private Dictionary<string, UIPanel> _panels = new();
        private Stack<UIPanel> _panelStack = new();

        #endregion


        #region Unity Methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseTopPanel();
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
                Debug.LogWarning($"패널 키{key}가 이미 등록되어있습니다.");
            }
        }

        public void Show(string key)
        {
            if (_panels.TryGetValue(key, out UIPanel panel))
            {
                if (_panelStack.Contains(panel)) return;    // 이미 스택에 있는 경우 return

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

        }

        public void Hide(string key)
        {
            if (_panels.TryGetValue(key, out UIPanel panel))
            {
                if (!_panelStack.Contains(panel)) return;   // 스택에 없는 경우 return

                panel.HideAnimation();
            }
        }

        public void CloseTopPanel()
        {
            if (_panelStack.Count > 1)
            {
                UIPanel topPanel = _panelStack.Pop();
                UIPanel previousPanel = _panelStack.Peek();

                if (topPanel != null)
                {
                    topPanel.HideAnimation();
                }

                if (previousPanel != null)
                {
                    previousPanel.ShowAnimation();
                }
            }
        }
        public void Clear() { }

        #endregion



        #region Private Methods
        #endregion
    }
}
