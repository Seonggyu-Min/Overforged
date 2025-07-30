using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    public class PlayerRecordBoxItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private TMP_Text _winRateText;
        [SerializeField] private TMP_Text _winLoseDrawCountText;

        public void SetPlayerRecord(string nickName, float winRate, int winCount, int loseCount, int drawCount)
        {
            _nickNameText.text = nickName;
            _winRateText.text = $"{winRate * 100:F2}%";
            _winLoseDrawCountText.text = $"{winCount}승 {loseCount}패 {drawCount}무";
        }
    }
}
