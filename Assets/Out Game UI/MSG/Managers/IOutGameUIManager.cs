using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIN
{
    public interface IOutGameUIManager
    {
        public void RegisterPanel(string key, UIPanel panel);

        public void Show(string key);

        public void Hide(string key);

        public void ShowAsFirst(string key);

        public void CloseTopPanel();

        public void Clear();
    }
}
