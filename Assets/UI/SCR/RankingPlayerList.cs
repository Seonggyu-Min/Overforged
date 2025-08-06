using System.Collections;
using System.Collections.Generic;
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

        public void SetPlayer()
        {

        }

        private void SetRank(int value)
        {
            if (value < 4)
            {
                rankingText.color = textColor.Color[value - 1];
                medalImage.sprite = medalList.sprites[value - 1];
            }
            rankingText.text = $"{value}";
        }
    }
}
