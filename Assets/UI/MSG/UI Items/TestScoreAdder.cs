using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class TestScoreAdder : MonoBehaviour
    {
        [Inject] IScoreManager _scoreManager;
        [Inject] IGameManager _gameManager;


        public void OnClickScoreAddButton()
        {
            _scoreManager.AddScore(PhotonNetwork.LocalPlayer, 10);
        }

        public void OnClickEndGameButton()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                _gameManager.SetGameEnd();
            }
            else
            {
                Debug.Log("게임 종료는 마스터 클라이언트만 수행할 수 있습니다.");
            }
        }

        public void OnClickCalculateWinLoseButton()
        {
            CalculatedPlayer calculatedPlayer = _gameManager.CalculateResult();

            foreach (var p in calculatedPlayer.WinPlayers)
            {
                if (p == PhotonNetwork.LocalPlayer)
                {
                    // 승리 UI
                }

                Debug.Log($"승리한 플레이어: {p.NickName}");
            }

            foreach (var p in calculatedPlayer.LosePlayers)
            {
                if (p == PhotonNetwork.LocalPlayer)
                {
                    // 패배 UI
                }

                Debug.Log($"패배한 플레이어: {p.NickName}");
            }

            foreach (var p in calculatedPlayer.DrawPlayers)
            {
                if (p == PhotonNetwork.LocalPlayer)
                {
                    // 무승부 UI
                }

                Debug.Log($"무승부 플레이어: {p.NickName}");
            }
            Debug.Log("승패 계산 요청을 보냈습니다.");
        }
    }
}

