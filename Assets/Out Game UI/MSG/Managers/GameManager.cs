using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class GameManager : MonoBehaviour ,IGameManager
    {
        [Inject] IFirebaseManager _firebaseManager;


        #region Public Methods

        /// <summary>
        /// 게임이 끝나고 방으로 되돌아가기 위해 호출하는 메서드.
        /// 마스터 클라이언트만 해당 메서드를 호출해야합니다.
        /// </summary>
        public void SetGameEnd()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                CalculateResult();

                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
                PhotonNetwork.LoadLevel(0);
            }
            else
            {
                Debug.LogWarning("게임 종료는 마스터 클라이언트만 수행할 수 있습니다.");
            }
        }

        // TODO: 만약 1대1구조가 아니라면, 추후 승패 관련 로직을 수정해야 함.
        private void CalculateResult()
        {
            var playerList = PhotonNetwork.PlayerList;

            if (playerList.Length == 0)
            {
                Debug.LogWarning("플레이어가 없습니다.");
                return;
            }

            // 점수 딕셔너리 구성
            Dictionary<Player, int> scores = new();

            foreach (var player in playerList)
            {
                int score = 0;

                if (player.CustomProperties.TryGetValue(CustomPropertyKeys.Score, out var value))
                {
                    score = (int)value;
                }
                else
                {
                    score = 0;
                }

                scores[player] = score;
            }

            int maxScore = scores.Values.Max();
            var topScorers = scores.Where(pair => pair.Value == maxScore).Select(pair => pair.Key).ToList();

            // 승자가 한 명일 경우
            if (topScorers.Count == 1)
            {
                foreach (var pair in scores)
                {
                    if (pair.Key == topScorers[0])
                    {
                        WinPlayer(pair.Key);
                    }
                    else
                    {
                        LosePlayer(pair.Key);
                    }
                }
            }
            // 승자가 여러 명이면서 전부가 아닐 경우
            else if (topScorers.Count > 1 && topScorers.Count < playerList.Length)
            {
                foreach (var pair in scores)
                {
                    if (topScorers.Contains(pair.Key))
                    {
                        WinPlayer(pair.Key);
                    }
                    else
                    {
                        LosePlayer(pair.Key);
                    }
                }
            }
            // 승자가 전부일 경우
            else if (topScorers.Count == playerList.Length)
            {
                foreach (var player in playerList)
                {
                    DrawPlayer(player);
                }
            }
            else
            {
                Debug.LogWarning("예상치 못한 결과입니다.");
            }
        }

        #endregion


        #region Private Methods

        private void WinPlayer(Player player)
        {
            if (player.UserId == null)
            {
                Debug.Log("player.UserId 가 null");
            }
            else
            {
                Debug.Log($"player.UserId 가 {player.UserId}");
            }


            var statRef = _firebaseManager.Database.RootReference.Child("users")
                .Child(player.UserId)
                .Child("stats")
                .Child("win");

            statRef.RunTransaction(mutableData =>
            {
                if (mutableData.Value == null)
                {
                    mutableData.Value = 1;
                }
                else
                {
                    mutableData.Value = (long)mutableData.Value + 1;
                }

                return TransactionResult.Success(mutableData);
            });
        }

        private void LosePlayer(Player player)
        {
            if (player.UserId == null)
            {
                Debug.Log("player.UserId 가 null");
            }
            else
            {
                Debug.Log($"player.UserId 가 {player.UserId}");
            }


            var statRef = _firebaseManager.Database.RootReference.Child("users")
                .Child(player.UserId)
                .Child("stats")
                .Child("lose");

            statRef.RunTransaction(mutableData =>
            {
                if (mutableData.Value == null)
                {
                    mutableData.Value = 1;
                }
                else
                {
                    mutableData.Value = (long)mutableData.Value + 1;
                }

                return TransactionResult.Success(mutableData);
            });
        }

        private void DrawPlayer(Player player)
        {
            var statRef = _firebaseManager.Database.RootReference.Child("users")
                .Child(player.UserId)
                .Child("stats")
                .Child("draw");

            statRef.RunTransaction(mutableData =>
            {
                if (mutableData.Value == null)
                {
                    mutableData.Value = 1;
                }
                else
                {
                    mutableData.Value = (long)mutableData.Value + 1;
                }

                return TransactionResult.Success(mutableData);
            });
        }

        #endregion
    }
}
