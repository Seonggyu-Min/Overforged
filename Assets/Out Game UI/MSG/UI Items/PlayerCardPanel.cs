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

        [SerializeField] SCR.SelectToggle characterToggle;
        [SerializeField] SCR.SelectToggle teamToggle;
        [SerializeField] private Button _readyButton;
        [SerializeField] private TMP_Text _readyButtonText;
        private bool _isReady = false;

        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _debugText;

        private Dictionary<int, PlayerCardItem> _playerCardDict = new();

        #endregion


        #region Unity Methods

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            ResetReadyState();
            ResetCustomProperties();
            UpdatePlayerCards();
            UpdateButtonText();
            _infoText.text = string.Empty;
        }

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
            ResetReadyState();
            ResetCustomProperties();
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
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { CustomPropertyKeys.CharacterId, characterToggle.SelectIndex },
                { CustomPropertyKeys.TeamColor, teamToggle.SelectIndex }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            UpdateButtonText();

            // 마스터 클라이언트일 경우 시작 기능
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터 클라이언트가 시작 버튼을 클릭한 경우
                //if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                //{
                //    _infoText.text = "최소 2명 이상이 필요합니다.";
                //    return;
                //}

                // 모든 플레이어가 준비 상태인지 확인
                if (PhotonNetwork.PlayerList.Length != 1)
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.IsMasterClient) continue; // 마스터 클라이언트는 제외
                        if (!player.CustomProperties.TryGetValue(CustomPropertyKeys.IsReady, out object isReadyObj) || !(bool)isReadyObj)
                        {
                            _infoText.text = "모든 플레이어가 준비 상태여야 합니다.";
                            return;
                        }
                    }
                }

                // 팀이 최소 2개 이상인지 확인
                if (PhotonNetwork.PlayerList.Length != 1)
                {
                    HashSet<int> teamColors = new HashSet<int>();

                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object teamColorObj) && teamColorObj is int teamColorId)
                        {
                            teamColors.Add(teamColorId);
                        }
                    }

                    if (teamColors.Count < 2)
                    {
                        _infoText.text = "최소 2개의 팀이 필요합니다.";
                        return;
                    }
                }

                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                int mapId = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomPropertyKeys.MapId, out object mapIdObj) && mapIdObj is int id ? id : 0;

                PhotonNetwork.LoadLevel(1);
            }
            // 일반 플레이어일 경우 준비 상태 토글
            else
            {
                _isReady = !_isReady;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { CustomPropertyKeys.IsReady, _isReady } });
            }
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

        private void ResetCustomProperties()
        {
            var props = PhotonNetwork.LocalPlayer.CustomProperties;

            bool hasCharacter = props.TryGetValue(CustomPropertyKeys.CharacterId, out object charObj);
            bool hasTeamColor = props.TryGetValue(CustomPropertyKeys.TeamColor, out object colorObj);

            // 이미 캐릭터와 팀 색상이 설정되어 있다면 아무것도 하지 않음
            if (hasCharacter && hasTeamColor) return;

            int chosenCharacterId = GetRandomCharacterId();
            int chosenColorId = GetRandomTeamColor();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
                { CustomPropertyKeys.CharacterId, chosenCharacterId },
                { CustomPropertyKeys.TeamColor, chosenColorId }
            });
        }

        private int GetRandomCharacterId()
        {
            return Random.Range(0, System.Enum.GetValues(typeof(CharacterId)).Length);
        }

        private int GetRandomTeamColor()
        {
            int[] allIds = (int[])System.Enum.GetValues(typeof(TeamColorId)); // 모든 팀 색상 ID 가져오기
            HashSet<int> usedIds = new(); // 이미 사용된 팀 색상 ID 저장

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object value))
                {
                    usedIds.Add((int)value); // 사용된 팀 색상 ID 추가
                }
            }

            List<int> available = new(); // 사용 가능한 팀 색상 ID 저장
            foreach (int id in allIds)
            {
                if (!usedIds.Contains(id)) // 사용되지 않은 팀 색상 ID만 추가
                {
                    available.Add(id);
                }
            }

            return available.Count > 0
                ? available[Random.Range(0, available.Count)] // 사용 가능한 색상이 있으면 그 중에서 선택
                : allIds[Random.Range(0, allIds.Length)]; // 없으면 아무거나 선택
        }

        private void ResetReadyState()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    { CustomPropertyKeys.IsReady, false }
                });
                _isReady = false;
            }
        }
    }

    #endregion
}
