using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace SCR
{
    public class WaitingUI : MonoBehaviour
    {
        [SerializeField] List<LoadingPlayerUI> playerUIs;
        [SerializeField] GameObject popUp;
        [SerializeField] Animator waitingAnimator;
        private List<bool> isConnected;

        public void setPlayer(int playerNum)
        {
            isConnected = new();
            for (int i = 0; i < playerUIs.Count; i++)
            {
                if (i < playerNum)
                {
                    playerUIs[i].gameObject.SetActive(true);
                    isConnected.Add(false);
                }

            }
        }

        public void ConnectedPlayer(int index)
        {
            playerUIs[index].Connect();
            isConnected[index] = true;
        }

        public void DisconnectedPlayer(int index)
        {
            playerUIs[index].Disconnect();
            isConnected[index] = false;
        }

        public bool AllConnectedPlayer()
        {
            foreach (bool connect in isConnected)
                if (!connect) return false;
            return true;
        }

        public IEnumerator GameStart()
        {
            popUp.SetActive(false);
            waitingAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(false);
        }
    }

}
