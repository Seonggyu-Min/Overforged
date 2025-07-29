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
    }
}

