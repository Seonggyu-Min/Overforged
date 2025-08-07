using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class MenuPanel : MonoBehaviour
    {
        [Inject] private IOutGameUIManager _outGameUIManager;
        [SerializeField] private GameObject _menuPopUp;
        [SerializeField] private GameObject _userInfoPopUp;
        private bool _isMenuPopUpShown = false;
        private bool _isUserInfoPopUpShown = false;

        private void OnEnable()
        {
            _isMenuPopUpShown = false;
            _isUserInfoPopUpShown = false;
            _menuPopUp.SetActive(_isMenuPopUpShown);
        }

        public void OnClickLeaderBoardButton()
        {
            _outGameUIManager.Show("Leader Board Panel");
        }

        public void OnToggleMenuPopUp()
        {
            _isMenuPopUpShown = !_isMenuPopUpShown;
            _menuPopUp.SetActive(_isMenuPopUpShown);
        }

        public void OnToggleUserInfoPopUp()
        {
            _isUserInfoPopUpShown = !_isUserInfoPopUpShown;
            _userInfoPopUp.SetActive(_isUserInfoPopUpShown);
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
