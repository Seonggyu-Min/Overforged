using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MIN
{
    public class CustomPropertyToggleSetter : MonoBehaviour
    {
        [Header("커스텀 프로퍼티 키")]
        public string propertyKey;

        [Header("적용할 Enum의 int 값")]
        public int propertyValue;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool isOn)
        {
            if (!isOn) return;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { propertyKey, propertyValue }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
}
