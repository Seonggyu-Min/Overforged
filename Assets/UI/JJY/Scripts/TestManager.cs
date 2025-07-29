using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;
using Photon.Realtime;

namespace JJY
{
    // 게임 시작하는 Manager에 붙이면 될듯.
    public class GameTimer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private float totalTime = 120f;

        private double startTime;
        private bool isRunning = false;

        private const string StartTimeKey = "StartTime";

        // void Awake() // 테스트용
        // {
        //     PhotonNetwork.ConnectUsingSettings();
        //     PhotonNetwork.JoinLobby();
        //     PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions(), TypedLobby.Default);
        //     PhotonNetwork.OfflineMode = true;
        // }

        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartGameTimer();
            }
            else
            {
                TryGetStartTimeFromRoom();
            }
        }

        void Update()
        {
            if (!isRunning) return;

            double elapsed = PhotonNetwork.Time - startTime;
            double remaining = Mathf.Max(0f, (float)(totalTime - elapsed));

            TimeSpan ts = TimeSpan.FromSeconds(remaining);
            timerText.text = $"{ts.Minutes}:{ts.Seconds}";

            if (remaining <= 0f)
            {
                isRunning = false;
                timerText.text = "00:00";
                Debug.Log("타이머 종료");

                // TODO : 타임 종료 UI 생성
            }
        }

        public void StartGameTimer()
        {
            startTime = PhotonNetwork.Time;

            // 방장이 CustomProperties에 시작시간 저장
            Hashtable props = new Hashtable { { StartTimeKey, startTime } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            isRunning = true;
        }

        private void TryGetStartTimeFromRoom()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(StartTimeKey, out object startTimeObj))
            {
                startTime = (double)startTimeObj;
                isRunning = true;
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(StartTimeKey))
            {
                TryGetStartTimeFromRoom();
            }
        }
    }
}
