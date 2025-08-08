using Photon.Pun;
using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;

namespace SCR
{
    public class CurrentTimeUI : MonoBehaviourPun
    {
        [SerializeField] InGameUIManager inGameUIManager;
        [SerializeField] TMP_Text leftTimeText;
        [SerializeField] float leftTime;
        [SerializeField] Color nomalColor;
        [SerializeField] Color lastColor;

        private Coroutine startTimeCO;
        private Coroutine lastTimeCO;

        private void OnDisable()
        {
            if (startTimeCO != null)
            {
                StopCoroutine(startTimeCO);
                startTimeCO = null;
            }
            if (lastTimeCO != null)
            {
                StopCoroutine(lastTimeCO);
                lastTimeCO = null;
            }
        }

        public void StartTime()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (startTimeCO != null) return;

                startTimeCO = StartCoroutine(StartTimeCor());
            }
        }

        private IEnumerator StartTimeCor()
        {
            while (leftTime > 0)
            {
                photonView.RPC("SyncTime", RpcTarget.All, leftTime);
                yield return new WaitForSeconds(1f);
                leftTime--;
            }
            photonView.RPC("EndGameAction", RpcTarget.All);
        }

        [PunRPC]
        public void SyncTime(float syncedTime)
        {
            SetTime(syncedTime);
        }

        [PunRPC]
        public void EndGameAction()
        {
            inGameUIManager.EndGameAction?.Invoke();
        }


        private void SetTime(float time)
        {
            leftTimeText.text = $"{(int)time / 60:D2}:{(int)time % 60:D2}";
        }

        public void LastChance()
        {
            photonView.RPC("SyncTime", RpcTarget.All, 0);
            leftTimeText.color = lastColor;
            lastTimeCO = StartCoroutine(LastChanceCor());
        }

        private IEnumerator LastChanceCor()
        {
            bool isOn = true;
            while (!inGameUIManager.IsEnd)
            {
                leftTimeText.gameObject.SetActive(isOn);
                yield return new WaitForSeconds(1f);
                isOn = !isOn;
            }
        }
    }
}

