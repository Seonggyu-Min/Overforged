using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MIN
{
    public class CreateRoomPopUpPanelBehaviour : MonoBehaviourPun
    {
        #region Fields and Properties

        [Inject] private IOutGameUIManager _outGameUIManager;

        [Header("방 생성 관련")]
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        [Header("방 인원 수 관련")]
        [SerializeField] private TMP_Text _maxPlayerText;
        [SerializeField] private Button _maxPlayerPlusButton;
        [SerializeField] private Button _maxPlayerMinusButton;
        private int _maxPlayerCount = 2;
        private const int MAX_PLAYER_COUNT = 8;
        private const int MIN_PLAYER_COUNT = 2;

        [Header("비밀 방 관련")]
        [SerializeField] private TMP_InputField _passWordInputField;
        [SerializeField] private Toggle _secretRoomToggle;

        [Header("안내 문구")]
        [SerializeField] private PopupUIBehaviour _popup;

        #endregion


        #region Unity Methods

        private void OnEnable() => Init();

        #endregion


        #region Public Methods

        public void OnClickConfirmButton() => Confirm();

        public void OnClickCancelButton() => _outGameUIManager.CloseTopPanel();

        public void OnClickPlusButton() => PlusMaxPlayer();

        public void OnClickMinusButton() => MinusMaxPlayer();

        public void OnClickSecretButton() => CheckSecretRoom();

        #endregion


        #region Private Methods

        private void Init()
        {
            _maxPlayerCount = 2;
            _maxPlayerText.text = _maxPlayerCount.ToString();

            // 비밀방 관련 UI 초기화
            _secretRoomToggle.isOn = false;
            _passWordInputField.gameObject.SetActive(false);

            ClearText();
        }

        private void Confirm()
        {
            if (string.IsNullOrWhiteSpace(_roomNameInputField.text))
            {
                _popup.ShowInfo("방 이름을 입력해주세요.");
                return;
            }

            if (_secretRoomToggle.isOn)
            {
                if (string.IsNullOrEmpty(_passWordInputField.text))
                {
                    _popup.ShowInfo("비밀 방의 비밀번호를 입력해주세요.");
                    return;
                }
                if (_passWordInputField.text.Length < 4)
                {
                    _popup.ShowInfo("비밀 방의 비밀번호는 4자리 이상이어야 합니다.");
                    return;
                }
                if (_passWordInputField.text.Length > 16)
                {
                    _popup.ShowInfo("비밀 방의 비밀번호는 16자리 이하이어야 합니다.");
                    return;
                }
            }

            Hashtable customProperties = new Hashtable
            {
                { CustomPropertyKeys.IsSecret, _secretRoomToggle.isOn },
                { CustomPropertyKeys.Password, _secretRoomToggle.isOn ? _passWordInputField.text : string.Empty }
            };

            if (_secretRoomToggle.isOn)
            {
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = (byte)_maxPlayerCount,
                    PublishUserId = true,
                    //IsOpen = true,
                    //IsVisible = true,
                    CustomRoomProperties = customProperties,
                    CustomRoomPropertiesForLobby = new string[] { CustomPropertyKeys.IsSecret, CustomPropertyKeys.Password },
                };

                PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions);
            }
            else
            {
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = (byte)_maxPlayerCount,
                    //IsOpen = true,
                    //IsVisible = true,
                    PublishUserId = true
                };

                PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions);
            }

            _outGameUIManager.CloseTopPanel();
            _outGameUIManager.Hide("Lobby Panel", () =>
            {
                _outGameUIManager.Show("Room Panel");
            });
        }

        private void PlusMaxPlayer()
        {
            _maxPlayerCount = Mathf.Min(_maxPlayerCount + 1, MAX_PLAYER_COUNT);
            _maxPlayerText.text = _maxPlayerCount.ToString();
        }

        private void MinusMaxPlayer()
        {
            _maxPlayerCount = Mathf.Max(_maxPlayerCount - 1, MIN_PLAYER_COUNT);
            _maxPlayerText.text = _maxPlayerCount.ToString();
        }

        private void CheckSecretRoom()
        {
            if (_secretRoomToggle.isOn)
            {
                _passWordInputField.gameObject.SetActive(true);
            }
            else
            {
                _passWordInputField.gameObject.SetActive(false);
            }
        }

        private void ClearText()
        {
            _roomNameInputField.text = string.Empty;
            _passWordInputField.text = string.Empty;
        }

        #endregion
    }
}
