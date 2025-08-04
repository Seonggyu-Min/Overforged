using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class ScoreStatusUser : MonoBehaviour
    {
        [SerializeField] TeamColor color;
        [SerializeField] Image teamColor;
        [SerializeField] Image playerImage;
        [SerializeField] TMP_Text playerNickname;
        [SerializeField] TMP_Text playerScore;

        public void SetPlayer(Sprite profile, string Nickname, int team)
        {
            playerImage.sprite = profile;
            playerNickname.text = Nickname;
            teamColor.color = color.Color[team];
            SetScore(0);
        }

        public void SetScore(int Score)
        {
            playerScore.text = $"{Score}";
        }

        public void SetResult(string Nickname, int team, int Score)
        {
            playerNickname.text = Nickname;
            teamColor.color = color.Color[team];
            SetScore(Score);
        }
    }
}
