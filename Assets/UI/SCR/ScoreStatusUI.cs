using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SCR
{
    public class ScoreStatusUI : MonoBehaviour
    {
        [SerializeField] GameObject popupUI;
        [SerializeField] List<ScoreStatusUser> playerUIs;

        public void setPlayer(int playerNum)
        {
            for (int i = 0; i < playerUIs.Count; i++)
            {
                if (i < playerNum)
                {
                    playerUIs[i].gameObject.SetActive(true);
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
