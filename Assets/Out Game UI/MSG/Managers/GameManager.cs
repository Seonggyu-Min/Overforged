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
    public enum GameResult
    {
        Win,
        Lose,
        Draw
    }

    /// <summary>
    /// 승리팀, 패배팀, 무승부팀을 저장하는 리스트. 
    /// 승리, 패배, 무승부팀에도 여러 팀이 들어갈 수 있으므로 List<List<Player>> 형태로 저장
    /// </summary>
    public class CalculatedTeam
    {

        public List<List<Player>> WinTeams = new();
        public List<List<Player>> LoseTeams = new();
        public List<List<Player>> DrawTeams = new();
    }

    public class GameManager : MonoBehaviour, IGameManager
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
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
                PhotonNetwork.LoadLevel(0);
            }
            else
            {
                Debug.LogWarning("게임 종료는 마스터 클라이언트만 수행할 수 있습니다.");
            }
        }

        /// <summary>
        /// 게임이 승패 계산을 위해 호출하는 메서드.
        /// 모든 클라이언트가 해당 메서드를 호출해야합니다.
        /// </summary>
        public CalculatedTeam CalculateResult()
        {
            // 혼자 봇과 1대1
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                // 봇의 점수를 알아야 됨 -> 어떻게?

                // 내 점수 - 봇 점수 > 봇 점수 => WinPlayer(PhotonNetwork.LocalPlayer)
            }

            // 봇과 2대1
            else if (PhotonNetwork.PlayerList.Length == 3)
            {
                // 이건 그냥 계산해도 될 듯
                // 근데 내 점수 - 봇 점수를 내 커스텀 플레이어 프로퍼티에 덮어씌워줘야 원래 점수가 나올 듯

                // 확장성 있게 하려면 ( PhotonNetwork.PlayerList.Length % 2 != 0 )일 때 팀원과 팀을 저장해서 누구에게 봇이있는지 확인해야될 듯 
            }

            CalculatedTeam calculatedTeam = new();
            var playerList = PhotonNetwork.PlayerList;

            if (playerList.Length == 0)
            {
                Debug.LogWarning("플레이어가 없습니다.");
                return null;
            }

            // 팀별 점수 계산
            GetTeamScoreData(out var teamGroups, out var teamScores);

            if (teamScores.Count == 0)
            {
                Debug.LogWarning("팀 점수가 없습니다.");
                return null;
            }

            // 최대 점수를 가진 팀을 찾기
            int maxScore = teamScores.Values.Max();
            // 최대 점수를 가진 팀의 키를 찾기
            var topTeams = teamScores.Where(pair => pair.Value == maxScore).Select(pair => pair.Key).ToList();

            // 팀별로 승패를 계산
            foreach (var kvp in teamGroups)
            {
                var teamKey = kvp.Key;
                var teamPlayers = kvp.Value;

                // topTeams에 포함된 팀인지 확인
                if (topTeams.Contains(teamKey))
                {
                    // 만약 모든 팀이 동점이라면
                    if (topTeams.Count == teamScores.Count)
                    {
                        // 무승부팀에 넣기
                        calculatedTeam.DrawTeams.Add(teamPlayers);
                    }
                    // 만약 동점이 아니라면 승리 처리
                    else
                    {
                        // 승리팀에 넣기
                        calculatedTeam.WinTeams.Add(teamPlayers);
                        foreach (var player in teamPlayers)
                        {
                            WinPlayer(player);
                        }
                    }
                }
                // 만약 topTeams에 포함되지 않은 팀이라면 패배 처리
                else
                {
                    // 패배팀에 넣기
                    calculatedTeam.LoseTeams.Add(teamPlayers);
                }
            }

            return calculatedTeam;
        }


        // TODO: 중복된 로직이 있어 리팩토링 필요
        /// <summary>
        /// 승패 저장을 위한 메서드
        /// 마스터 클라이언트만 호출해야 합니다.
        /// </summary>
        public void SaveTeamResult()
        {
            var playerList = PhotonNetwork.PlayerList;

            if (playerList.Length == 0)
            {
                Debug.LogWarning("플레이어가 없습니다.");
                return;
            }

            // 여기도 봇 관련 승패 저장 로직 필요


            // 팀별 점수 계산
            GetTeamScoreData(out var teamGroups, out var teamScores);

            if (teamScores.Count == 0)
            {
                Debug.LogWarning("팀 점수가 없습니다.");
                return;
            }

            // 최대 점수를 가진 팀을 찾기
            int maxScore = teamScores.Values.Max();
            // 최대 점수를 가진 팀의 키를 찾기
            var topTeams = teamScores.Where(pair => pair.Value == maxScore).Select(pair => pair.Key).ToList();

            // 팀별로 승패를 계산
            foreach (var kvp in teamGroups)
            {
                var teamKey = kvp.Key;
                var teamPlayers = kvp.Value;

                // topTeams에 포함된 팀인지 확인
                if (topTeams.Contains(teamKey))
                {
                    // 만약 모든 팀이 동점이라면
                    if (topTeams.Count == teamScores.Count)
                    {
                        // 무승부 처리
                        foreach (var player in teamPlayers)
                        {
                            DrawPlayer(player);
                        }
                    }
                    // 만약 동점이 아니라면 승리 처리
                    else
                    {
                        // 승리 처리
                        foreach (var player in teamPlayers)
                        {
                            WinPlayer(player);
                        }
                    }
                }
                // 만약 topTeams에 포함되지 않은 팀이라면 패배 처리
                else
                {
                    // 패배 처리
                    foreach (var player in teamPlayers)
                    {
                        LosePlayer(player);
                    }
                }
            }
        }

        /// <summary>
        /// TopTeams에 여러 팀이 있는지 확인하는 메서드.
        /// True면 최고 점수 팀이 2개 이상입니다.
        /// </summary>
        public bool IsTieForWinTeam()
        {
            var playerList = PhotonNetwork.PlayerList;

            if (playerList.Length == 0)
            {
                Debug.LogWarning("플레이어가 없습니다.");
                return false;
            }

            // 팀 점수 계산
            GetTeamScoreData(out _, out var teamScores);

            if (teamScores.Count == 0)
            {
                Debug.LogWarning("팀 점수가 없습니다.");
                return false;
            }

            int maxScore = teamScores.Values.Max();
            int topTeamCount = teamScores.Values.Count(score => score == maxScore);

            return topTeamCount > 1;
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

        private void GetTeamScoreData(out Dictionary<object, List<Player>> teamGroups, out Dictionary<object, int> teamScores)
        {
            teamGroups = new(); // key: 팀 색상, value: 해당 팀의 플레이어 리스트
            teamScores = new(); // key: 팀 색상, value: 해당 팀의 총 점수

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!player.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out var teamKey))
                {
                    Debug.LogWarning($"플레이어 {player.NickName}의 팀 색상이 설정되지 않았습니다.");
                    continue;
                }

                // 팀 색상 키가 없으면 새로 생성
                if (!teamGroups.ContainsKey(teamKey))
                {
                    teamGroups[teamKey] = new List<Player>();
                    teamScores[teamKey] = 0;
                }

                // 팀 그룹에 플레이어 추가
                teamGroups[teamKey].Add(player);

                // 플레이어의 점수를 가져와서 해당 팀의 총 점수에 추가
                int score = 0;
                if (player.CustomProperties.TryGetValue(CustomPropertyKeys.Score, out var scoreValue))
                {
                    score = (int)scoreValue;
                }

                // 해당 팀의 총 점수에 플레이어의 점수를 추가
                teamScores[teamKey] += score;
            }
        }

        #endregion
    }
}
