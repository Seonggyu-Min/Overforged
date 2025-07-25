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
    public class RoomCardPanel : MonoBehaviourPunCallbacks
    {
        [Inject] private DiContainer _diContainer;

        [SerializeField] private GameObject _roomCardPrefab;
        [SerializeField] private Transform _roomContentParent;

        [SerializeField] private Button _leftArrowButton;
        [SerializeField] private Button _rightArrowButton;
        [SerializeField] private TMP_Text _roomPageText;

        private List<RoomInfo> _roomList = new();
        private int _currentPage = 0;
        private const int RoomsPerPage = 6;


        public void SetRoomList(List<RoomInfo> list)
        {
            _roomList = list;
            int totalPages = Mathf.CeilToInt((float)_roomList.Count / RoomsPerPage);
            _currentPage = Mathf.Clamp(_currentPage, 0, Mathf.Max(totalPages - 1, 0));
            UpdateRoomCards();
        }


        public void UpdateRoomCards()
        {
            // RoomCard 모두 제거
            foreach (Transform child in _roomContentParent)
            {
                Destroy(child.gameObject);
            }

            // 표기할 방의 시작 인덱스와 끝 인덱스 계산
            int startIndex = _currentPage * RoomsPerPage;
            int endIndex = Mathf.Min(startIndex + RoomsPerPage, _roomList.Count);

            // 현재 페이지에 해당하는 방 카드 생성
            for (int i = startIndex; i < endIndex; i++)
            {
                RoomCardItem roomCardItem = _diContainer.InstantiatePrefabForComponent<RoomCardItem>(_roomCardPrefab, _roomContentParent);
                roomCardItem.SetInfo(_roomList[i]);
            }

            // 페이지 텍스트 업데이트
            _roomPageText.text = $"Page {_currentPage + 1} / {Mathf.CeilToInt((float)_roomList.Count / RoomsPerPage)}";
        }


        public void OnClickLeftArrowButton()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdateRoomCards();
            }
        }

        public void OnClickRightArrowButton()
        {
            if ((_currentPage + 1) * RoomsPerPage < _roomList.Count)
            {
                _currentPage++;
                UpdateRoomCards();
            }
        }
    }
}
