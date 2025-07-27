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
    public class PlayerCardPanel : MonoBehaviourPunCallbacks
    {
        #region Fields And Properties

        [Inject] private DiContainer _diContainer;
        [Inject] private IOutGameUIManager _outGameUIManager;

        [SerializeField] private GameObject _playerCardPrefab;
        [SerializeField] private Transform _playerContentParent;

        [SerializeField] private Button _readyButton;
        [SerializeField] private TMP_Text _readyButtonText;
        private bool _isReady = false;

        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _debugText;

        private Dictionary<int, PlayerCardItem> _playerCardDict = new();

        #endregion


        #region Unity Methods

        public override void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
        
        public override void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

        // 디버그용
        private void Update()
        {
            _debugText.text = $"Player Count: {PhotonNetwork.PlayerList.Length}\n" +
                              $"Is Master Client: {PhotonNetwork.IsMasterClient}\n" +
                              $"Is In Room: {PhotonNetwork.InRoom}\n" +
                              $"Is Connected: {PhotonNetwork.IsConnected}\n" +
                              $"PlayerActorNum: {PhotonNetwork.LocalPlayer.ActorNumber}";
        }

        #endregion


        #region Photon Callbacks

        public override void OnJoinedRoom()
        {
            UpdatePlayerCards();
            UpdateButtonText();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (_playerCardDict.TryGetValue(targetPlayer.ActorNumber, out PlayerCardItem card))
            {
                card.UpdateStatus(targetPlayer);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdatePlayerCards();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdatePlayerCards();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            UpdatePlayerCards();
            UpdateButtonText();
        }

        #endregion


        #region Public Methods

        public void OnClickReadyOrStartButton()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터 클라이언트가 시작 버튼을 클릭한 경우
                if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    _infoText.text = "최소 2명 이상이 필요합니다.";
                    return;
                }

                // 모든 플레이어가 준비 상태인지 확인
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.IsMasterClient) continue; // 마스터 클라이언트는 제외
                    if (!player.CustomProperties.TryGetValue(CustomPropertyKeys.IsReady, out object isReadyObj) || !(bool)isReadyObj)
                    {
                        _infoText.text = "모든 플레이어가 준비 상태여야 합니다.";
                        return;
                    }
                }

                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel("GameScene");
            }
            else
            {
                // 일반 플레이어가 준비 상태를 토글함
                _isReady = !_isReady;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { CustomPropertyKeys.IsReady, _isReady } });
            }

            UpdateButtonText();
        }

        public void OnClickExitButton()
        {
            _outGameUIManager.CloseTopPanel();
        }

        #endregion


        #region Private Methods

        private void UpdateButtonText()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _readyButtonText.text = "Start";
            }
            else
            {
                if (_isReady)
                {
                    _readyButtonText.text = "Unready";
                }
                else
                {
                    _readyButtonText.text = "Ready";
                }
            }
        }


        private void UpdatePlayerCards()
        {
            // 현재 방에 존재하는 플레이어 ActorNumber 저장
            HashSet<int> currentActors = new();

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                currentActors.Add(player.ActorNumber);

                if (_playerCardDict.TryGetValue(player.ActorNumber, out PlayerCardItem existingCard))
                {
                    // 이미 존재하는 카드가 있으면 업데이트
                    existingCard.UpdateStatus(player);
                }
                else
                {
                    // 새로운 플레이어 카드 생성
                    PlayerCardItem newCard = _diContainer.InstantiatePrefabForComponent<PlayerCardItem>(_playerCardPrefab, _playerContentParent);
                    newCard.Init(player);
                    _playerCardDict[player.ActorNumber] = newCard;
                }
            }

            // 방에서 나간 플레이어 카드 제거
            List<int> toRemove = new();
            foreach (var kvp in _playerCardDict)
            {
                if (!currentActors.Contains(kvp.Key))
                {
                    Destroy(kvp.Value.gameObject);
                    toRemove.Add(kvp.Key);
                }
            }

            // 제거할 플레이어 카드 딕셔너리에서 제거
            foreach (int actorNumber in toRemove)
            {
                _playerCardDict.Remove(actorNumber);
            }
        }

        #endregion
    }
}
