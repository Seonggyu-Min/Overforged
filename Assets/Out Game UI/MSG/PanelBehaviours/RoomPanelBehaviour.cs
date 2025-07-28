using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    public class RoomPanelBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text _roomNameText;


        public override void OnEnable()
        {
            StartCoroutine(RoomNameUpdateRoutine());
        }

        private IEnumerator RoomNameUpdateRoutine()
        {
            float timeout = 3f;
            float timer = 0f;

            while (PhotonNetwork.CurrentRoom == null && timer < timeout)
            {
                timer += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            _roomNameText.text = PhotonNetwork.CurrentRoom != null
                ? PhotonNetwork.CurrentRoom.Name
                : "방 없음";
        }
    }
}
