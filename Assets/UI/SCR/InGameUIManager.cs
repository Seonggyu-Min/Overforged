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
<<<<<<< HEAD
        private string Score = "Score";
=======
>>>>>>> Develop

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

            foreach (var p in PhotonNetwork.PlayerList)
            {
                players.Add(p);
            }

            EndGameAction += CheckFinish;

            waitingUI.setPlayer(players.Count);
            scoreStatusUI.setPlayer(players);
            IsWaiting = true;
            IsEnd = false;
            IsLastChance = false;
        }

        private void Start()
        {
<<<<<<< HEAD
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
=======
            //foreach (var p in players)
            //{
            //    if ((bool)p.CustomProperties[localSceneLoaded] == true)
            //    {
            //        Debug.Log($"{p.NickName}의 준비 상태가 true");
            //        waitingUI.ConnectedPlayer(CheckPlayerIndex(p));
            //    }
            //}
>>>>>>> Develop
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
<<<<<<< HEAD
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            if (IsWaiting)
            {
=======
            Debug.Log("OnPlayerPropertiesUpdate Entered");
            if (IsWaiting)
            {
                Debug.Log("IsWaiting");
>>>>>>> Develop
                if (changedProps.ContainsKey(CustomPropertyKeys.localSceneLoaded))
                {
                    Debug.Log("changedProps.ContainsKey(localSceneLoaded)");
                    // 플레이어가 게임에 진입한 경우
<<<<<<< HEAD
                    bool join = (bool)changedProps[CustomPropertyKeys.localSceneLoaded];
                    if (join)
                    {
                        waitingUI.ConnectedPlayer(CheckPlayerIndex(targetPlayer));
                        if (waitingUI.AllConnectedPlayer())
                            if (PhotonNetwork.IsMasterClient)
                                photonView.RPC("AllConnected", RpcTarget.All);
=======
                    waitingUI.ConnectedPlayer(CheckPlayerIndex(targetPlayer));
                    Debug.Log($"{targetPlayer.NickName}");

                    if (waitingUI.AllConnectedPlayer())
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            photonView.RPC("AllConnected", RpcTarget.AllBuffered);
                        }
>>>>>>> Develop
                    }
                }
            }
            if (!IsWaiting)
            {
                if (changedProps.ContainsKey(CustomPropertyKeys.Score))
                {
                    int score = (int)changedProps[CustomPropertyKeys.Score];
                    scoreStatusUI.ChangeScore(CheckPlayerIndex(targetPlayer), score);

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
            if (GameManager.IsTieForWinTeam())
            {
                LastChance();
                IsLastChance = true;
            }
            else
            {
                EndGame();
                IsLastChance = false;
            }
        }

        // 점수가 같다면 실행할 함수
        private void LastChance()
        {
            currentTimeUI.LastChance();
        }


        // 게임이 끝났을 경우 실행할 함수
        private void EndGame()
        {
            resultUI.gameObject.SetActive(true);
            resultUI.OnResultUI(players);
        }


    }
}

