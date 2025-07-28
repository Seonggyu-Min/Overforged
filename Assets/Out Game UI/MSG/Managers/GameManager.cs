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
    // TODO: 1대1이 아닐 경우, 봇이 존재할 경우 구조를 수정해야됨
    public class MatchPlayerData
    {
        public Player MyPlayer;
        //public Player OpponentPlayer; // TODO: 추후 전적 보기 등을 위해 여러 필드를 추가할 수 있음
        public int Score;
    }

    public class GameManager : MonoBehaviour ,IGameManager
    {
        [Inject] IFirebaseManager _firebaseManager;


        #region Public Methods

        /// <summary>
        /// 게임이 끝나고 방으로 되돌아가기 위해 호출하는 메서드.
        /// 마스터 클라이언트만 해당 메서드를 호출해야합니다.
        /// </summary>
        public void GoToRoom()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
                PhotonNetwork.LoadLevel(0);
            }
        }

        // TODO: 만약 1대1구조가 아니라면, 추후 승패 관련 로직을 수정해야 함.

        /// <summary>
        /// 게임 결과를 계산하고 승패를 결정 및 저장합니다.
        /// 마스터 클라이언트만 해당 메서드를 호출해야합니다.
        /// </summary>
        /// <param name="result">MatchPlayerData로 플레이어 데이터와 점수를 넘겨 승패를 결정합니다</param>
        public void CalculateResult(List<MatchPlayerData> result)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("마스터 클라이언트만 결과를 계산할 수 있습니다.");
                return;
            }

            if (result == null || result.Count == 0)
            {
                Debug.LogWarning("플레이어 데이터가 없습니다.");
                return;
            }

            int maxScore = result.Max(data => data.Score);
            var topScorers = result.Where(data => data.Score == maxScore).ToList();

            if (topScorers.Count == 1)
            {
                // 단독 승자
                foreach (var data in result)
                {
                    if (data.Score == maxScore)
                    {
                        WinPlayer(data.MyPlayer);
                    }
                    else
                    {
                        LosePlayer(data.MyPlayer);
                    }
                }
            }
            else if (topScorers.Count > 1 && topScorers.Count < result.Count)
            {
                // 동점자 다수일 경우 여러 명이 승자
                foreach (var data in result)
                {
                    if (data.Score == maxScore)
                    {
                        WinPlayer(data.MyPlayer);
                    }
                    else
                    {
                        LosePlayer(data.MyPlayer);
                    }
                }
            }
            else if (topScorers.Count == result.Count)
            {
                // 모두 동점일 경우
                foreach (var data in result)
                {
                    DrawPlayer(data.MyPlayer);
                }
            }
            else
            {
                Debug.LogWarning("예상치 못한 결과입니다. 플레이어 수와 점수 데이터가 일치하지 않습니다.");
            }
        }

        #endregion


        #region Private Methods

        private void WinPlayer(Player player)
        {
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
