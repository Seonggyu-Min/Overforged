using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SCR
{
    public class ScoreStatusUI : MonoBehaviour
    {
        [SerializeField] InGameUIManager inGameUIManager;
        [SerializeField] GameObject popupUI;
        [SerializeField] List<ScoreStatusUser> playerUIs;

        public void setPlayer(List<Photon.Realtime.Player> players)
        {
            for (int i = 0; i < playerUIs.Count; i++)
            {
                if (i < players.Count)
                {
                    playerUIs[i].gameObject.SetActive(true);
                    playerUIs[i].SetPlayer(null, players[i].NickName,
                    int.Parse(players[i].CustomProperties["TeamColor"].ToString()));
                }
            }
        }

        public void ChangeScore(int index, int score)
        {
            playerUIs[index].SetScore(score);
        }

        private void OnSeeScore(InputValue value)
        {
            Debug.Log("켜기");
            popupUI.gameObject.SetActive(value.isPressed);
        }
    }
}
