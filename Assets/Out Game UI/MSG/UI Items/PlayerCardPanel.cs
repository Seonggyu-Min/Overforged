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
        [Inject] private DiContainer _diContainer;

        [SerializeField] private GameObject _playerCardPrefab;
        [SerializeField] private Transform _playerContentParent;

        [SerializeField] private Button _readyButton;
        [SerializeField] private TMP_Text _readyButtonText;
        private bool _isReady = false;

        [SerializeField] private TMP_Text _infoText;


        public override void OnEnable()
        {
            InitButtonText();
            UpdatePlayerCards();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            UpdatePlayerCards();
        }

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
                PhotonNetwork.CurrentRoom.IsOpen = false; // 방을 닫음
                PhotonNetwork.CurrentRoom.IsVisible = false; // 방을 비공개로 설정
                PhotonNetwork.LoadLevel("GameScene"); // 게임 씬 로드
            }
            else
            {
                // 일반 플레이어가 준비 상태를 토글함
                _isReady = !_isReady;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { CustomPropertyKeys.IsReady, _isReady } });
            }
        }

        private void InitButtonText()
        {
            _readyButtonText.text = PhotonNetwork.IsMasterClient ? "Start" : "Ready";
        }

        private void UpdatePlayerCards()
        {
            _readyButtonText.text = _isReady ? "Unready" : "Ready";

            // PlayerCard 모두 제거
            foreach (Transform child in _playerContentParent)
            {
                Destroy(child.gameObject);
            }

            // 현재 플레이어의 카드 생성
            foreach (var player in PhotonNetwork.PlayerList)
            {
                PlayerCardItem playerCardItem = _diContainer.InstantiatePrefabForComponent<PlayerCardItem>(_playerCardPrefab, _playerContentParent);
                playerCardItem.Init(player);
            }
        }
    }
}
