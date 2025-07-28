using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    public class ChatBoxItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageText;

        public void Init(string sender, string message)
        {
            _messageText.text = $"{sender}: {message}";
        }
    }
}
