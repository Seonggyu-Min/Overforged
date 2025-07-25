using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    public class PlayerCardItem : MonoBehaviourPunCallbacks
    {
        [Inject] private IFirebaseManager _firebaseManager;

        [SerializeField] private TMP_Text _nicknameText;
        [SerializeField] private TMP_Text _masterOrReadyText;
        [SerializeField] private Image _characterImage;
        [SerializeField] private Image _crownImage;


        public void Init(Player player)
        {
            _nicknameText.text = player.NickName;
            _crownImage.gameObject.SetActive(player.IsMasterClient);

            bool isReady = false;
            if (player.CustomProperties.TryGetValue(CustomPropertyKeys.IsReady, out object readyObj))
            {
                isReady = (bool)readyObj;
            }

            if (player.IsMasterClient)
            {
                _masterOrReadyText.text = "Master";
            }
            else
            {
                if (!isReady)
                {
                    _masterOrReadyText.text = "Not Ready";
                }
                else
                {
                    _masterOrReadyText.text = "Ready";
                }
            }
        }
    }
}
