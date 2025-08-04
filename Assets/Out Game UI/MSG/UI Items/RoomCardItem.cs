using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class RoomCardItem : MonoBehaviour
    {
        [Inject] private IOutGameUIManager _uiManager;

        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private TMP_Text _roomPlayerCountText;
        [SerializeField] private GameObject _statusPanel;
        [SerializeField] private GameObject _lockImage;

        [SerializeField] private Sprite[] _mapSprites; // 맵 스프라이트들 등록  TODO: MapData SO를 공유하면 좋을 듯?
        [SerializeField] private Image _mapImage;

        private RoomInfo _roomInfo;


        public void SetInfo(RoomInfo info)
        {
            _roomInfo = info;

            _roomNameText.text = info.Name;
            _roomPlayerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
            _statusPanel.SetActive(!info.IsOpen);
            
            if (_roomInfo.CustomProperties.TryGetValue(CustomPropertyKeys.Password, out object storedPasswordObj) &&
                    storedPasswordObj is string)
                _lockImage.SetActive(true);

            if (_roomInfo.CustomProperties.TryGetValue(CustomPropertyKeys.MapId, out object mapIdObj) && mapIdObj is int mapId)
            {
                if (mapId >= 0 && mapId < _mapSprites.Length)
                {
                    _mapImage.sprite = _mapSprites[mapId];
                }
                else
                {
                    Debug.LogWarning($"MapId {mapId}가 _mapSprites 범위를 벗어났습니다.");
                }
            }
            else
            {
                Debug.LogWarning("MapId 커스텀 속성이 설정되지 않아 기본값 사용 또는 표시 생략");
            }
        }

        public void OnClickThisButton()
        {
            if (_roomInfo == null)
            {
                Debug.LogWarning("룸 정보가 설정되지 않았습니다.");
            }

            if (_roomInfo != null && _roomInfo.PlayerCount < _roomInfo.MaxPlayers)
            {
                if (_roomInfo.CustomProperties.TryGetValue(CustomPropertyKeys.Password, out object storedPasswordObj) &&
                    storedPasswordObj is string)
                {
                    var panel = _uiManager.GetPanel("Password Input Panel");
                    var passwordInputPanel = panel.GetComponent<PasswordInputPanelBehaviour>();
                    passwordInputPanel.Init(_roomInfo);

                    _uiManager.Show("Password Input Panel");
                }
                else
                {
                    _uiManager.Hide("Lobby Panel", () =>
                    {
                        _uiManager.Show("Room Panel");
                        PhotonNetwork.JoinRoom(_roomInfo.Name);
                    });
                }
            }
            else
            {
                Debug.LogWarning("방에 참여할 수 없습니다. 방이 가득 찼거나 비공개 방입니다.");
            }
        }
    }
}
