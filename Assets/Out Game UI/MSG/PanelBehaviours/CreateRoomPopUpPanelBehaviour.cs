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
        [SerializeField] private TMP_Text _roomPasswordText;
        [SerializeField] private TMP_InputField _passWordInputField;
        [SerializeField] private Button _secretRoomCheckBoxButton;
        [SerializeField] private Sprite _secretTrueSprite;
        [SerializeField] private Sprite _secretFalseSprite;
        private bool _isSecretRoom = false;

        [Header("안내 문구")]
        [SerializeField] private TMP_Text _infoText;

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
            _isSecretRoom = false;
            _secretRoomCheckBoxButton.image.sprite = _secretFalseSprite;
            _passWordInputField.gameObject.SetActive(false);
            _roomPasswordText.gameObject.SetActive(false);

            ClearText();
        }

        private void Confirm()
        {
            if (string.IsNullOrWhiteSpace(_roomNameInputField.text))
            {
                _infoText.text = "방 이름을 입력해주세요.";
                return;
            }

            if (_isSecretRoom)
            {
                if (string.IsNullOrEmpty(_passWordInputField.text))
                {
                    _infoText.text = "비밀 방의 비밀번호를 입력해주세요.";
                    return;
                }
                if (_passWordInputField.text.Length < 4)
                {
                    _infoText.text = "비밀 방의 비밀번호는 4자리 이상이어야 합니다.";
                    return;
                }
                if (_passWordInputField.text.Length > 16)
                {
                    _infoText.text = "비밀 방의 비밀번호는 16자리 이하이어야 합니다.";
                    return;
                }
            }

            Hashtable customProperties = new Hashtable
            {
                { CustomPropertyKeys.IsSecret, _isSecretRoom },
                { CustomPropertyKeys.Password, _isSecretRoom ? _passWordInputField.text : string.Empty }
            };

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = (byte)_maxPlayerCount,
                CustomRoomProperties = customProperties,
                CustomRoomPropertiesForLobby = new string[] { CustomPropertyKeys.IsSecret, CustomPropertyKeys.Password },
            };

            PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions);

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
            _isSecretRoom = !_isSecretRoom;

            if (_isSecretRoom)
            {
                _secretRoomCheckBoxButton.image.sprite = _secretTrueSprite;
                _passWordInputField.gameObject.SetActive(true);
                _roomPasswordText.gameObject.SetActive(true);
            }
            else
            {
                _secretRoomCheckBoxButton.image.sprite = _secretFalseSprite;
                _passWordInputField.gameObject.SetActive(false);
                _roomPasswordText.gameObject.SetActive(false);
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
