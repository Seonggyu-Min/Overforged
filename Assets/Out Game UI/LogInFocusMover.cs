using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MIN
{
    public class LogInFocusMover : MonoBehaviour
    {
        [SerializeField] private TMP_InputField[] _inputFields;
        [SerializeField] private LogInPanelBehaviour _logInPanel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SelectNextField();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleEnterKey();
            }
        }

        private void SelectNextField()
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            int currentIndex = GetCurrentInputFieldIndex(current);

            int nextIndex = (currentIndex + 1) % _inputFields.Length;
            _inputFields[nextIndex].ActivateInputField();
        }

        private void HandleEnterKey()
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            int currentIndex = GetCurrentInputFieldIndex(current);

            if (currentIndex == _inputFields.Length - 1)
            {
                _logInPanel.OnClickLoginButton(); // 마지막 필드에서 Enter 눌렀을 때만 로그인 실행
            }
            else
            {
                SelectNextField(); // 아니면 그냥 다음 필드로 이동
            }
        }

        private int GetCurrentInputFieldIndex(GameObject current)
        {
            for (int i = 0; i < _inputFields.Length; i++)
            {
                if (_inputFields[i].gameObject == current)
                {
                    return i;
                }
            }
            return -1; // 못 찾으면 -1
        }
    }
}
