using Firebase.Database;
using Firebase.Extensions;
using MIN;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SCR
{
    public class ResultUI : MonoBehaviour
    {
        [Inject] IGameManager gameManager;
        [Inject] IFirebaseManager firebaseManager;

        [SerializeField] List<Sprite> titleSprite;
        [SerializeField] List<Color> shadowColor;
        [SerializeField] List<GameObject> titleDeco;
        [SerializeField] Image shadow;
        [SerializeField] Image title;
        [SerializeField] TMP_Text titleText;

        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text goldText;
        [SerializeField] TMP_Text expText;

        [SerializeField] List<ScoreStatusUser> otherPlayers;
        [SerializeField] TeamColor color;
        [SerializeField] List<Image> teamColor;

        [SerializeField] float closeTime;

        bool isWin;

        private void OnEnable()
        {
            CheckLocalWin();
            SetScore();
            AddOtherPlayer(); // 테스트용 호출
        }

        public void Win()
        {
            title.sprite = titleSprite[0];
            shadow.color = shadowColor[0];
            titleDeco[0].SetActive(true);
            titleDeco[1].SetActive(false);
            //titleDeco[2].SetActive(false);
            titleText.text = "WIN";
            isWin = true;
        }

        public void Lose()
        {
            title.sprite = titleSprite[1];
            shadow.color = shadowColor[1];
            titleDeco[0].SetActive(false);
            titleDeco[1].SetActive(true);
            //titleDeco[2].SetActive(false);
            titleText.text = "LOSE";
            isWin = false;
        }

        private void Draw()
        {
            title.sprite = titleSprite[2];
            shadow.color = shadowColor[2];
            titleDeco[0].SetActive(false);
            titleDeco[1].SetActive(false);
            //titleDeco[2].SetActive(true);
            titleText.text = "DRAW";
            isWin = false;
        }

        public void SetScore()
        {
            int team = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.TeamColor];
            int score = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.Score];

            teamColor[0].color = color.Color[team];
            teamColor[1].color = color.Color[team];

            scoreText.text = $"{score}";
            if (isWin)
            {
                //goldText.text = $"{50 + score / 1000}";
                expText.text = $"{score * 10}";
            }
            else
            {
                //goldText.text = $"{25 + score / 1000}";
                expText.text = $"{score * 5}";
            }

            SaveExpToFirebase(score);
        }



        /// <summary>
        /// 다른 플레이어의 닉네임, 팀 컬러, 점수 표기.
        /// InGameUIManager에서 호출해야되는 메서드입니다.
        /// </summary>
        public void AddOtherPlayer(List<Photon.Realtime.Player> players)
        {
            players.RemoveAll(p => p == PhotonNetwork.LocalPlayer);

            int displayCount = Mathf.Min(players.Count, otherPlayers.Count);

            for (int i = 0; i < displayCount; i++)
            {
                otherPlayers[i].gameObject.SetActive(true);
                otherPlayers[i].SetResult(
                    players[i].NickName,
                    (int)players[i].CustomProperties[CustomPropertyKeys.TeamColor],
                    (int)players[i].CustomProperties[CustomPropertyKeys.Score]
                );
            }

            // 남은 슬롯 비활성화
            for (int i = displayCount; i < otherPlayers.Count; i++)
            {
                otherPlayers[i].gameObject.SetActive(false);
            }

            // 나머지 칸은 비활성화
            for (int i = players.Count; i < otherPlayers.Count; i++)
            {
                otherPlayers[i].gameObject.SetActive(false);
            }
        }

        // 테스트용 메서드
        // 우선 테스트용으로 자기가 불러오기
        // 원래는 중간에 나간 사람도 표기가 되어야하기 때문에 InGameUIManager에서 플레이어 리스트를 전달해야 됨
        public void AddOtherPlayer()
        {
            List<Photon.Realtime.Player> players = new(PhotonNetwork.PlayerListOthers);
            players.RemoveAll(p => p == PhotonNetwork.LocalPlayer);

            // 자기 자신은 otherPlayers에 업데이트 되지 않도록 삭제
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == PhotonNetwork.LocalPlayer)
                {
                    players.Remove(players[i]);
                }
            }

            int displayCount = Mathf.Min(players.Count, otherPlayers.Count);

            for (int i = 0; i < displayCount; i++)
            {
                otherPlayers[i].gameObject.SetActive(true);
                otherPlayers[i].SetResult(
                    players[i].NickName,
                    (int)players[i].CustomProperties[CustomPropertyKeys.TeamColor],
                    (int)players[i].CustomProperties[CustomPropertyKeys.Score]
                );
            }

            for (int i = displayCount; i < otherPlayers.Count; i++)
            {
                otherPlayers[i].gameObject.SetActive(false);
            }
        }

        private IEnumerable CloseTab()
        {
            yield return new WaitForSeconds(closeTime);
            gameObject.SetActive(false);
        }

        // 로컬 플레이어의 승리 체크
        private void CheckLocalWin()
        {
            CalculatedTeam calculatedTeams = gameManager.CalculateResult();

            foreach (var p in calculatedTeams.WinTeams)
            {
                foreach (var player in p)
                {
                    if (player == PhotonNetwork.LocalPlayer)
                    {
                        Win();
                    }
                }
            }

            foreach (var p in calculatedTeams.LoseTeams)
            {
                foreach (var player in p)
                {
                    if (player == PhotonNetwork.LocalPlayer)
                    {
                        Lose();
                    }
                }
            }

            foreach (var p in calculatedTeams.DrawTeams)
            {
                foreach (var player in p)
                {
                    if (player == PhotonNetwork.LocalPlayer)
                    {
                        Draw();
                    }
                }
            }
        }

        // Firebase에 경험치 저장
        private void SaveExpToFirebase(int exp)
        {
            string uid = firebaseManager.Auth.CurrentUser.UserId;
            DatabaseReference userRef = firebaseManager.Database.GetReference("users").Child(uid);

            userRef.Child("exp").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                DataSnapshot snapshot = task.Result;

                if (task.IsCompletedSuccessfully)
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogWarning("경험치 접근이 취소됨");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogWarning($"경험치 접근에 실패함. 이유 : {task.Exception}");
                        return;
                    }

                    int currentExp = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;
                    int newExp = currentExp + exp;

                    userRef.Child("exp").SetValueAsync(newExp).ContinueWithOnMainThread(
                        setTask =>
                        {
                            if (setTask.IsCompletedSuccessfully)
                            {
                                Debug.Log($"경험치가 성공적으로 저장됨{newExp}");
                            }
                            else
                            {
                                Debug.LogWarning("경험치 저장에 실패함");
                            }
                        });
                }
                else
                {
                    Debug.LogWarning("현재 경험치 불러오기 실패");
                }
            });
        }
    }
}
