using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class PasswordInputPanelBehaviour : MonoBehaviour
    {
        [Inject] private IOutGameUIManager _uiManager;

        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_Text _infoText;

        private RoomInfo _roomInfo;


        public void Init(RoomInfo info)
        {
            _passwordInputField.text = string.Empty;
            _roomInfo = info;
        }

        public void OnClickConfirmButton()
        {
            if (string.IsNullOrEmpty(_passwordInputField.text))
            {
                _infoText.text = "비밀번호를 입력해주세요.";
            }
            else if (_passwordInputField.text != _roomInfo.CustomProperties[CustomPropertyKeys.Password].ToString())
            {
                _infoText.text = "비밀번호가 일치하지 않습니다.";
            }
            else
            {
                _infoText.text = "비밀번호가 일치합니다.";

                PhotonNetwork.JoinRoom(_roomInfo.Name);

                _uiManager.CloseTopPanel();
                _uiManager.Hide("Lobby Panel", () =>
                {
                    _uiManager.Show("Room Panel");
                });
            }
        }

        public void OnClickCancelButton()
        {
            _uiManager.CloseTopPanel();
        }
    }
}
