using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace MIN
{
    public class ScoreIndicator : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text _scoreText;

        private void Awake()
        {
            if (_scoreText == null)
            {
                _scoreText = GetComponent<TMP_Text>();
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (targetPlayer != PhotonNetwork.LocalPlayer) return;

            if (targetPlayer.CustomProperties.TryGetValue(CustomPropertyKeys.Score, out var score))
            {
                _scoreText.text = $"{targetPlayer.NickName}: {score}";
            }
            else
            {
                _scoreText.text = $"{targetPlayer.NickName}: 0";
            }
        }
    }
}
