using Firebase.Database;
using Firebase.Extensions;
using SCR;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MIN
{

    public class LeaderBoardPanel : MonoBehaviour
    {
        [Inject] private IOutGameUIManager _outGameUIManager;

        private class PlayerStatData
        {
            public string Nickname;
            public int Rank;
            public int Level;
            public int Win;
            public int Lose;
            public int Draw;

            public float WinRate => (Win + Lose) == 0 ? 0 : (float)Win / (Win + Lose);
            public int Total => Win + Lose + Draw;
        }


        [Inject] private IFirebaseManager _firebaseManager;

        [Header("My Leader Board")]
        [SerializeField] private RankingPlayerList _myRecord;

        [Header("Leader Board Slots (Must Be 10)")]
        [SerializeField] private List<RankingPlayerList> _playerRecordBoxItems;

        private List<PlayerStatData> _sortedPlayers = new();
        private int _currentPage = 0;
        private const int PLAYERS_PER_PAGE = 10;


        private void OnEnable()
        {
            LoadLeaderBoard();
            _currentPage = 0;
        }


        public void OnClickNextPage()
        {
            int maxPage = Mathf.CeilToInt(_sortedPlayers.Count / (float)PLAYERS_PER_PAGE);
            if (_currentPage < maxPage - 1)
            {
                _currentPage++;
                UpdateLeaderBoardPage();
            }
        }

        public void OnClickPrevPage()
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdateLeaderBoardPage();
            }
        }

        public void OnClickCloseButton()
        {
            _outGameUIManager.CloseTopPanel();
        }

        private void MyLeaderBoard()
        {
            string uid = _firebaseManager.Auth.CurrentUser.UserId;
            DatabaseReference userRef = _firebaseManager.Database.GetReference("users").Child(uid);

            userRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogWarning("유저 정보 불러오기가 취소됨");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogWarning($"유저 정보 불러오기에 실패함. 이유 : {task.Exception}");
                        return;
                    }

                    var snapshot = task.Result;

                    string nickname = snapshot.Child("nickname").Value?.ToString() ?? "Unknown";

                    _myRecord.SetPlayer(0, 0, nickname, 0, 0, 0);
                }
                else
                {
                    Debug.LogWarning("사용자 정보 불러오기 실패");
                }
            });
        }


        // TODO: 현재는 전부 가져와서 정렬하는 방식인데 더 효율적인 방법이 있다면 바꿀 듯
        private void LoadLeaderBoard()
        {
            string uid = _firebaseManager.Auth.CurrentUser.UserId;
            _firebaseManager.Database.GetReference("users").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("데이터베이스 정보를 가져오는 작업이 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"데이터베이스 정보를 가져오는 데 실패함. 이유: {task.Exception}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                _sortedPlayers.Clear();
                foreach (var userSnapshot in snapshot.Children)
                {
                    string nickname = userSnapshot.Child("nickname").Value?.ToString();

                    DataSnapshot stats = userSnapshot.Child("stats");
                    if (nickname == null || stats == null) continue;
                    int exp = int.TryParse(snapshot.Child("exp").Value?.ToString(), out int e) ? e : 0;
                    int level = exp / 100 + 1;
                    int win = int.TryParse(stats.Child("win").Value?.ToString(), out var w) ? w : 0;
                    int lose = int.TryParse(stats.Child("lose").Value?.ToString(), out var l) ? l : 0;
                    int draw = int.TryParse(stats.Child("draw").Value?.ToString(), out var d) ? d : 0;

                    _sortedPlayers.Add(new PlayerStatData
                    {
                        Nickname = nickname,
                        Level = level,
                        Win = win,
                        Lose = lose,
                        Draw = draw
                    });
                }

                // 승률 순 정렬
                _sortedPlayers.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));
                int rank = 1;
                string myNickName = _myRecord.getNickName();
                foreach (var p in _sortedPlayers)
                {
                    p.Rank = rank++;
                    if (p.Nickname == myNickName)
                    {
                        _myRecord.SetPlayer(0, p.Rank, p.Nickname, p.Level, p.WinRate, p.Total);
                    }
                }

                // 첫 페이지 보여주기
                _currentPage = 0;
                UpdateLeaderBoardPage();
            });
        }

        private void UpdateLeaderBoardPage()
        {
            int startIndex = _currentPage * PLAYERS_PER_PAGE;

            for (int i = 0; i < _playerRecordBoxItems.Count; i++)
            {
                if (startIndex + i < _sortedPlayers.Count)
                {
                    PlayerStatData data = _sortedPlayers[startIndex + i];
                    _playerRecordBoxItems[i].gameObject.SetActive(true);
                    _playerRecordBoxItems[i].SetPlayer(0, data.Rank, data.Nickname,
                     data.Level, data.WinRate, data.Total);
                }
                else
                {
                    _playerRecordBoxItems[i].gameObject.SetActive(false);
                }
            }

            int maxPage = Mathf.CeilToInt(_sortedPlayers.Count / (float)PLAYERS_PER_PAGE);
        }
    }
}

