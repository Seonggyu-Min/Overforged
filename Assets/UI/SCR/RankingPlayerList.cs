using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using SCR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCR
{

    public class RankingPlayerList : MonoBehaviour
    {
        [SerializeField] TeamColor textColor;
        [SerializeField] ImageList medalList;
        [SerializeField] ImageList profileList;

        [SerializeField] Image profileImage;
        [SerializeField] Image medalImage;
        [SerializeField] TMP_Text rankingText;
        [SerializeField] TMP_Text nickNameText;
        [SerializeField] TMP_Text LevelText;
        [SerializeField] TMP_Text RateText;
        [SerializeField] TMP_Text TotalGamesText;

        public void SetPlayer(int profile, int rank, string nickName, int level, float Rate, int TotalGames)
        {
            profileImage.sprite = profileList.sprites[profile];
            SetRank(rank);
            nickNameText.text = nickName;
            LevelText.text = $"Lv. {level}";
            RateText.text = $"{Rate * 100:F2}%";
            TotalGamesText.text = $"{TotalGames}";
        }

        public void SetRank(int value)
        {
            if (value < 4)
            {
                rankingText.color = textColor.Color[value - 1];
                medalImage.sprite = medalList.sprites[value - 1];
            }
            else
            {
                rankingText.color = textColor.Color[4];
                medalImage.sprite = medalList.sprites[4];
            }
            rankingText.text = $"{value}";
        }

        public string getNickName()
        {
            return nickNameText.text;
        }
    }
}
