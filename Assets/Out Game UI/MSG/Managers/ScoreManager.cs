using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIN
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        private void OnEnable()
        {
            ResetAllScore();
        }


        public void AddScore(Player player, int score)
        {
            if (player.CustomProperties.ContainsKey(CustomPropertyKeys.Score))
            {
                int currentScore = (int)player.CustomProperties[CustomPropertyKeys.Score];
                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { CustomPropertyKeys.Score, currentScore + score } });
            }
            else
            {
                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { CustomPropertyKeys.Score, score } });
            }
        }


        private void ResetAllScore()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                ResetScore(player);
            }
        }

        private void ResetScore(Player player)
        {
            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { CustomPropertyKeys.Score, 0 } });
        }
    }
}
