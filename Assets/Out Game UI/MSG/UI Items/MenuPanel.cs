using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPopUp;
        private bool _isMenuPopUpShown = false;

        private void OnEnable()
        {
            _isMenuPopUpShown = false;
            _menuPopUp.SetActive(_isMenuPopUpShown);
        }

        public void OnToggleMenuPopUp()
        {
            _isMenuPopUpShown = !_isMenuPopUpShown;
            _menuPopUp.gameObject.SetActive(_isMenuPopUpShown);
        }

        public void OnClickExitButton()
        {
            Exit();
        }


        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
