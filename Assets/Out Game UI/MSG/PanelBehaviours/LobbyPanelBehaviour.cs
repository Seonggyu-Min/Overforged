using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class LobbyPanelBehaviour : MonoBehaviourPunCallbacks
    {
        [Inject] private IOutGameUIManager _outGameUIManager;

        [SerializeField] private RoomCardPanel _roomCardPanel;

        [SerializeField] private Button _createRoomButton;

        private List<RoomInfo> _roomList = new();

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var info in roomList)
            {
                if (info.RemovedFromList)
                {
                    _roomList.RemoveAll(room => room.Name == info.Name);
                }
                else
                {
                    int index = _roomList.FindIndex(room => room.Name == info.Name);
                    if (index >= 0)
                    {
                        _roomList[index] = info; // 방 정보 업데이트
                    }
                    else
                    {
                        _roomList.Add(info);     // 새 방 추가
                    }
                }
            }

            _roomCardPanel.SetRoomList(_roomList);
        }


        public void OnClickCreateRoomButton()
        {
            _outGameUIManager.Show("Create Room PopUp Panel");
        }
    }
}
