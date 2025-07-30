using Firebase.Database;
using Firebase.Extensions;
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
            public int Win;
            public int Lose;
            public int Draw;

            public float WinRate => (Win + Lose) == 0 ? 0 : (float)Win / (Win + Lose);
        }


        [Inject] private IFirebaseManager _firebaseManager;

        [Header("Leader Board Slots (Must Be 10)")]
        [SerializeField] private List<PlayerRecordBoxItem> _playerRecordBoxItems;

        [Header("Page Info")]
        [SerializeField] private TMP_Text _pageInfoText;

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


        // TODO: 현재는 전부 가져와서 정렬하는 방식인데 더 효율적인 방법이 있다면 바꿀 듯
        private void LoadLeaderBoard()
        {
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

                    int win = int.TryParse(stats.Child("win").Value?.ToString(), out var w) ? w : 0;
                    int lose = int.TryParse(stats.Child("lose").Value?.ToString(), out var l) ? l : 0;
                    int draw = int.TryParse(stats.Child("draw").Value?.ToString(), out var d) ? d : 0;

                    _sortedPlayers.Add(new PlayerStatData
                    {
                        Nickname = nickname,
                        Win = win,
                        Lose = lose,
                        Draw = draw
                    });
                }

                // 승률 순 정렬
                _sortedPlayers.Sort((a, b) => b.WinRate.CompareTo(a.WinRate));

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
                    _playerRecordBoxItems[i].SetPlayerRecord(data.Nickname, data.WinRate, data.Win, data.Lose, data.Draw);
                }
                else
                {
                    _playerRecordBoxItems[i].gameObject.SetActive(false);
                }
            }

            int maxPage = Mathf.CeilToInt(_sortedPlayers.Count / (float)PLAYERS_PER_PAGE);
            _pageInfoText.text = $"{_currentPage + 1} / {Mathf.Max(maxPage, 1)} 페이지";
        }
    }
}

