using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{
    public class ScoreStatusUser : MonoBehaviour
    {
        [SerializeField] Image playerImage;
        [SerializeField] TMP_Text playerNickname;
        [SerializeField] TMP_Text playerScore;

        public void SetPlayer(Sprite profile, string Nickname)
        {
            playerImage.sprite = profile;
            playerNickname.text = Nickname;
            SetScore(0);
        }

        public void SetScore(int Score)
        {
            playerScore.text = $"{Score}";
        }
    }
}
