using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MIN
{
    public class MapSelectPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Image _mapImage;
        [SerializeField] private Sprite[] _mapSprites;

        private int _currentMapIndex = 0;

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnEnable()
        {
            // UI 패널의 Hide 이후 Show가 호출되기 때문에 먼저 방에 접속 후 SetActive(true) 되는 경우가 있어 OnEnable에서도 확인
            if (PhotonNetwork.InRoom)
            {
                _rightButton.enabled = PhotonNetwork.IsMasterClient;
                _leftButton.enabled = PhotonNetwork.IsMasterClient;
                ChangeMap();
            }
        }

        public override void OnJoinedRoom()
        {
            _rightButton.enabled = PhotonNetwork.IsMasterClient;
            _leftButton.enabled = PhotonNetwork.IsMasterClient;
            ChangeMap();
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(CustomPropertyKeys.MapId))
            {
                ChangeMap();
            }
        }


        public void OnClickRightButton()
        {
            _currentMapIndex++;
            if (_currentMapIndex >= _mapSprites.Length)
            {
                _currentMapIndex = 0;
            }

            ExitGames.Client.Photon.Hashtable roomProperty = new();
            roomProperty[CustomPropertyKeys.MapId] = _currentMapIndex;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperty);

            ChangeMap();
        }

        public void OnClickLeftButton()
        {
            _currentMapIndex--;
            if (_currentMapIndex < 0)
            {
                _currentMapIndex = _mapSprites.Length - 1;
            }

            ExitGames.Client.Photon.Hashtable roomProperty = new();
            roomProperty[CustomPropertyKeys.MapId] = _currentMapIndex;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperty);

            ChangeMap();
        }


        private void ChangeMap()
        {
            Debug.Log("ChangeMap 호출됨");

            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.LogWarning("ChangeMap 호출 시 CurrentRoom이 null입니다.");
                return;
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomPropertyKeys.MapId, out object mapIdObj) && mapIdObj is int mapId)
            {
                _currentMapIndex = mapId;
            }
            else
            {
                Debug.LogWarning("MapId 커스텀 속성이 존재하지 않아 기본값 0으로 설정합니다.");
                _currentMapIndex = 0;
            }

            if (_currentMapIndex >= 0 && _currentMapIndex < _mapSprites.Length)
            {
                _mapImage.sprite = _mapSprites[_currentMapIndex];
            }
            else
            {
                Debug.LogError($"MapIndex {_currentMapIndex}가 _mapSprites 범위를 벗어났습니다.");
            }
        }
    }
}
