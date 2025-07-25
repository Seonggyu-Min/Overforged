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
        [SerializeField] private TMP_Text _statusText;

        [SerializeField] private Image _mapImage;

        private RoomInfo _roomInfo;


        public void SetInfo(RoomInfo info)
        {
            _roomInfo = info;

            _roomNameText.text = info.Name;
            _roomPlayerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
            _statusText.text = info.IsOpen ? "Waiting" : "Playing";

            //_mapImage
        }

        public void OnClickThisButton()
        {
            if (_roomInfo == null)
            {
                Debug.LogWarning("룸 정보가 설정되지 않았습니다.");
            }

            if (_roomInfo != null && _roomInfo.PlayerCount < _roomInfo.MaxPlayers)
            {

                _uiManager.Hide("Room Card Panel", () =>
                {
                    _uiManager.Show("Room Panel");
                    PhotonNetwork.JoinRoom(_roomInfo.Name);
                });
            }
            else
            {
                Debug.LogWarning("방에 참여할 수 없습니다. 방이 가득 찼거나 비공개 방입니다.");
            }
        }
    }
}
