using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    /// <summary>
    /// SceneUIPanelRegistrar의 하위 오브젝트 중 UIPanel 컴포넌트를 가진 오브젝트들을 찾아 자동으로 등록해주는 컴포넌트입니다.
    /// </summary>
    public class SceneUIPanelRegistrar : MonoBehaviour
    {
        [Inject] private IOutGameUIManager _uiManager;

        private void Awake()
        {
            foreach (var panel in GetComponentsInChildren<UIPanel>(true))
            {
                string key = panel.name;
                UIPanel go = panel;

                _uiManager.RegisterPanel(key, go);
            }
        }
    }
}
