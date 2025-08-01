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
        [SerializeField] private TMP_Text _nicknameText;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private Image _crownImage;
        [SerializeField] private Image _characterImage;
        [SerializeField] private Image _teamColorImage;

        private int _actorNumber;


        public override void OnEnable() => PhotonNetwork.AddCallbackTarget(this);

        public override void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            // 플레이어의 속성이 업데이트되면 해당 플레이어 카드도 업데이트
            if (targetPlayer.ActorNumber == _actorNumber)
            {
                UpdateStatus(targetPlayer);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            UpdateStatus(newMasterClient);
        }


        public void Init(Player player)
        {
            _actorNumber = player.ActorNumber;
            _nicknameText.text = player.NickName;
            UpdateStatus(player);
        }


        public void UpdateStatus(Player player)
        {
            if (player == null)
            {
                Debug.LogWarning($"[PlayerCardItem] Null player for ActorNumber: {_actorNumber}");
                return;
            }

            _crownImage.gameObject.SetActive(player.IsMasterClient);


            if (!player.IsMasterClient)
            {
                bool isReady = false;
                if (player.CustomProperties.TryGetValue(CustomPropertyKeys.IsReady, out object readyObj))
                {
                    isReady = (bool)readyObj;
                }

                _readyPanel.SetActive(isReady);
            }

            UpdateCharacterImage(player);
            UpdateTeamColor(player);
        }


        private void UpdateCharacterImage(Player player)
        {
            if (player.CustomProperties.TryGetValue(CustomPropertyKeys.CharacterId, out object charIdObj))
            {
                int charId = (int)charIdObj;
                _characterImage.sprite = CustomPropertyDatabase.GetSpriteById((CharacterId)charId);
            }
        }

        private void UpdateTeamColor(Player player)
        {
            if (player.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object colorIdObj))
            {
                int colorId = (int)colorIdObj;
                _teamColorImage.color = CustomPropertyDatabase.GetColorById((TeamColorId)colorId);
            }
        }
    }
}
