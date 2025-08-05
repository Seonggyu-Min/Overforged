using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;

namespace SCR
{
    public class CurrentTimeUI : MonoBehaviour
    {
        [SerializeField] InGameUIManager inGameUIManager;
        [SerializeField] TMP_Text leftTimeText;
        [SerializeField] float leftTime;
        [SerializeField] Color nomalColor;
        [SerializeField] Color lastColor;

        public void StartTime()
        {
            StartCoroutine(StartTimeCor());
        }

        private IEnumerator StartTimeCor()
        {
            while (leftTime > 0)
            {
                SetTime(leftTime);
                yield return new WaitForSeconds(1f);
                leftTime--;
            }
            inGameUIManager.EndGameAction?.Invoke();
        }

        private void SetTime(float time)
        {
            leftTimeText.text = $"{(int)time / 60}:{(int)time % 60}";
        }

        public void LastChance()
        {
            StartCoroutine(LastChanceCor());
        }

        private IEnumerator LastChanceCor()
        {
            bool isOn = true;
            while (inGameUIManager.IsEnd)
            {
                leftTimeText.gameObject.SetActive(isOn);
                yield return new WaitForSeconds(1f);
                isOn = !isOn;
            }
        }
    }
}

