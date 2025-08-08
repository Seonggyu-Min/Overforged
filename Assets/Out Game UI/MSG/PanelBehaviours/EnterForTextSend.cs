using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class EnterForTextSend : MonoBehaviour
    {
        [SerializeField] private GlobalChatPanel _globalChatPanel;
        [SerializeField] private RoomChatPanel _roomChatPanel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_globalChatPanel != null)
                {
                    _globalChatPanel.OnClickSendButton();
                }
                else if (_roomChatPanel != null)
                {
                    _roomChatPanel.OnClickSendButton();
                }
            }
        }
    }
}
