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
    public class LobbyPanelBehaviour : MonoBehaviourPunCallbacks
    {
        #region Fields And Properties

        [Inject] private IOutGameUIManager _outGameUIManager;
        [Inject] private IFirebaseManager _firebaseManager;

        [SerializeField] private RoomCardPanel _roomCardPanel;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private GameObject _exitButton;
        [SerializeField] private MenuPanel _menuPanel;
        [SerializeField] private TMP_Text _debugText;


        private List<RoomInfo> _roomList = new();

        #endregion


        #region Unity Methods

        public override void OnEnable()
        {
            _exitButton.SetActive(true);
            _menuPanel.gameObject.SetActive(true);
            _menuPanel.TurnOnTutorial();
            PhotonNetwork.AddCallbackTarget(this);

            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("PhotonNetwork에 연결되지 않았습니다. 기다립니다.");
                StartCoroutine(ReEnterLobbyRoutine(false));
            }
            else if (!PhotonNetwork.InLobby)
            {
                Debug.Log("로비에 들어가지 않았습니다. 로비에 들어갑니다.");
                StartCoroutine(ReEnterLobbyRoutine(true));
            }

            // OnEnable이 JoinLobby 이후 실행되어서 콜백을 못 받음. 따라서 강제 새로고침 해봄
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            _exitButton.SetActive(false);
            _menuPanel.gameObject.SetActive(false);
        }

        // 디버그용
        private void Update()
        {
            _debugText.text = $"Player Count: {PhotonNetwork.PlayerList.Length}\n" +
                              $"Is Master Client: {PhotonNetwork.IsMasterClient}\n" +
                              $"Is In Room: {PhotonNetwork.InRoom}\n" +
                              $"Is In Lobby : {PhotonNetwork.InLobby}\n" +
                              $"Room List Count : {_roomList.Count}\n" +
                              $"Is Connected: {PhotonNetwork.IsConnected}\n" +
                              $"PhotonNetwork.NetworkClientState : {PhotonNetwork.NetworkClientState.ToString()}";
        }

        #endregion


        #region Photon Callbacks

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log($"OnRoomListUpdate 호출, 방 개수: {roomList.Count}");

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

        public override void OnJoinedLobby()
        {
            //_roomList.Clear(); // 기존 방 목록 초기화
            //_roomCardPanel.SetRoomList(_roomList); // 초기화된 방 목록 설정
        }

        #endregion


        #region Public Methods

        public void OnClickCreateRoomButton()
        {
            _outGameUIManager.Show("Create Room PopUp Panel");
        }

        public void OnClickRandomJoinButton()
        {
            List<RoomInfo> joinableRooms = new();

            foreach (RoomInfo room in _roomList)
            {
                if (room.IsOpen && room.IsVisible && room.PlayerCount < room.MaxPlayers)
                {
                    bool isSecret = room.CustomProperties.ContainsKey(CustomPropertyKeys.IsSecret) &&
                                    (bool)room.CustomProperties[CustomPropertyKeys.IsSecret];

                    if (!isSecret)
                    {
                        joinableRooms.Add(room);
                    }
                }
            }

            if (joinableRooms.Count == 0)
            {
                Debug.Log("참가 가능한 방이 없습니다.");
                return;
            }

            RoomInfo selectedRoom = joinableRooms[Random.Range(0, joinableRooms.Count)];

            _outGameUIManager.Hide("Lobby Panel", () =>
            {
                _outGameUIManager.Show("Room Panel");
                PhotonNetwork.JoinRoom(selectedRoom.Name);
            });
        }

        public void OnClickLogOutButton()
        {
            StartCoroutine(LogOutRoutine());
        }

        public void OnClickExitButton()
        {
            Exit();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// 로비에 다시 들어가기 위한 코루틴
        /// 이 방식이 맞는지 잘 모르겠음. 확인 후 리팩토링 해야될 수도
        /// </summary>
        private IEnumerator ReEnterLobbyRoutine(bool isConnected)
        {
            if (!isConnected)
            {
                yield return new WaitUntil(() => PhotonNetwork.IsConnected);
            }

            for (int i = 0; i < 5; i++) // 최대 5번 시도
            {
                if (PhotonNetwork.InLobby)
                {
                    Debug.Log("이미 로비에 있거나 로비 입장 중입니다.");
                    yield break;
                }

                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                }

                PhotonNetwork.JoinLobby();
                Debug.Log("로비 입장 시도");

                float elapsed = 0f;
                while (!PhotonNetwork.InLobby && elapsed < 2f) // 2초 대기
                {
                    yield return new WaitForSeconds(0.5f);
                    elapsed += 0.5f;
                }

                if (PhotonNetwork.InLobby)
                {
                    Debug.Log("로비 입장 성공");
                }
                else
                {
                    Debug.LogWarning("로비 입장 실패 또는 타임아웃");
                }
            }

            if (!PhotonNetwork.InLobby)
            {
                Debug.LogError("로비에 들어갈 수 없습니다. 다시 시도해주세요.");
            }
        }

        private IEnumerator LogOutRoutine()
        {
            // 1. Firebase 로그아웃
            _firebaseManager.Auth.SignOut();

            // 2. Photon 연결 끊기
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();

                // 연결 해제까지 대기
                yield return new WaitUntil(() =>
                    PhotonNetwork.NetworkClientState == ClientState.Disconnected);
            }

            // 3. UI 전환
            _outGameUIManager.Hide("Lobby Panel", () =>
            {
                _outGameUIManager.Show("Log In Panel");
            });
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion
    }
}
