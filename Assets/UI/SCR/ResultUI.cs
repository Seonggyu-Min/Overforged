using System.Collections;
using System.Collections.Generic;
using MIN;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SCR
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] InGameUIManager inGameUIManager;
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

        public void OnResultUI(List<Photon.Realtime.Player> players)
        {

        }

        private void Win()
        {
            title.sprite = titleSprite[0];
            shadow.color = shadowColor[0];
            titleDeco[0].SetActive(true);
            titleDeco[1].SetActive(false);
            titleText.text = "WIN";
            isWin = true;
        }

        private void Lose()
        {
            title.sprite = titleSprite[1];
            shadow.color = shadowColor[1];
            titleDeco[0].SetActive(false);
            titleDeco[1].SetActive(true);
            titleText.text = "LOSE";
            isWin = false;
        }

        private void SetScore(int team, int score)
        {
            teamColor[0].color = color.Color[team];
            teamColor[1].color = color.Color[team];
            scoreText.text = $"{score}";
            if (isWin)
            {
                goldText.text = $"{50 + score / 1000}";
                expText.text = $"{5 + score / 2000}";
            }
            else
            {
                goldText.text = $"{25 + score / 1000}";
                expText.text = $"{2 + score / 2000}";
            }
        }

        private void AddOtherPlayer(string Nickname, int team, int Score)
        {
            foreach (ScoreStatusUser player in otherPlayers)
            {
                if (!player.gameObject.activeSelf)
                {
                    player.SetResult(Nickname, team, Score);
                    player.gameObject.SetActive(true);
                    return;
                }
            }
        }

        private IEnumerator CloseTab()
        {
            yield return new WaitForSeconds(closeTime);
            gameObject.SetActive(false);
            inGameUIManager.GameManager.SaveTeamResult();
            inGameUIManager.GameManager.SetGameEnd();
        }

    }
}
