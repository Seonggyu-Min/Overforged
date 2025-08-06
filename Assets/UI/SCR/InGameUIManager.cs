using System;
using System.Collections.Generic;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using System.Linq;
using Zenject;
using MIN;
using Unity.VisualScripting;

namespace SCR
{
    public class InGameUIManager : MonoBehaviourPunCallbacks
    {
        private List<Photon.Realtime.Player> players;
        private string Score = "Score";

        [SerializeField] CurrentTimeUI currentTimeUI;
        [SerializeField] ScoreStatusUI scoreStatusUI;
        [SerializeField] WaitingUI waitingUI;
        public OrderUI OrderUI { get => orderUI; }
        [SerializeField] OrderUI orderUI;
        [SerializeField] ResultUI resultUI;
        [Inject] public IGameManager GameManager;

        public Action EndGameAction;
        public bool IsWaiting;
        public bool IsLastChance;
        public bool IsEnd;

        private void Awake()
        {
            players = new();
            players = PhotonNetwork.PlayerList.ToList();
            EndGameAction += CheckFinish;

            waitingUI.setPlayer(players.Count);
            scoreStatusUI.setPlayer(players);
            IsWaiting = true;
            IsEnd = false;
            IsLastChance = false;
        }

        private void Start()
        {
            foreach (var p in players)
            {
                Hashtable playerProps = p.CustomProperties;

                if (playerProps.ContainsKey(CustomPropertyKeys.localSceneLoaded))
                {
                    bool join = (bool)playerProps[CustomPropertyKeys.localSceneLoaded];
                    if (join)
                    {
                        waitingUI.ConnectedPlayer(CheckPlayerIndex(p));
                    }
                }
            }
            SendJoinMessage();
        }

        private void SendJoinMessage()
        {
            // 접속했다는 신호를 보냄
            Hashtable updatedProps = new()
            {
                { CustomPropertyKeys.localSceneLoaded, true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(updatedProps);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            if (IsWaiting)
            {
                if (changedProps.ContainsKey(CustomPropertyKeys.localSceneLoaded))
                {
                    // 플레이어가 게임에 진입한 경우
                    bool join = (bool)changedProps[CustomPropertyKeys.localSceneLoaded];
                    if (join)
                    {
                        waitingUI.ConnectedPlayer(CheckPlayerIndex(targetPlayer));
                        if (waitingUI.AllConnectedPlayer())
                            if (PhotonNetwork.IsMasterClient)
                                photonView.RPC("AllConnected", RpcTarget.All);
                    }
                }
            }
            if (!IsWaiting)
            {
                if (changedProps.ContainsKey(CustomPropertyKeys.Score))
                {
                    scoreStatusUI.ChangeScore(CheckPlayerIndex(targetPlayer), 1);
                    if (IsLastChance) CheckFinish();
                }
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            waitingUI.DisconnectedPlayer(CheckPlayerIndex(otherPlayer));
            // 아직 게임을 시작하지 않았고, 팀전이라면 게임 자동 종료
            if (IsWaiting)
            {
                IsEnd = true;
                EndGame();
            }

        }

        // 플레이어의 ActorNumber를 확인
        private int CheckPlayerIndex(Photon.Realtime.Player player)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (player.NickName == players[i].NickName)
                    return i;
            }
            return players.Count + 1;
        }

        // 모두가 접속이 완료되면 애니메이션 시작
        [PunRPC]
        public void AllConnected()
        {
            StartCoroutine(waitingUI.GameStart());
        }

        // 게임 시작 함수
        public void StartGame()
        {
            IsWaiting = false;
            currentTimeUI.StartTime();
            orderUI.StartOrder();
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject obj in taggedObjects)
            {
                obj.GetComponent<Player>().OnPlayerInput();
            }
        }

        // 끝났음을 확인하는 함수
        private void CheckFinish()
        {
            //if(GameManager.)
            // {
            // LastChance();
            //  IsLastChance = true;
            // }
            // 1, 2등의 점수가 같다면 LastChance 실행
            // else{
            // EndGame();
            //IsLastChance = false;
            //}
            // 1등이 2등보다 점수가 높다면 EndGame 실행
        }

        // 점수가 같다면 실행할 함수
        private void LastChance()
        {
            currentTimeUI.LastChance();
        }


        // 게임이 끝났을 경우 실행할 함수
        private void EndGame()
        {
            resultUI.OnResultUI(players);
            resultUI.gameObject.SetActive(true);
        }


    }
}

