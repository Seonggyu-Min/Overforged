using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace MIN
{
    [Serializable]
    public class ChatMessage
    {
        public string sender;
        public string text;
        public long timestamp;

        public ChatMessage(string sender, string text)
        {
            this.sender = sender;
            this.text = text;
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    public class GlobalChatPanel : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;

        [SerializeField] private List<ChatBoxItem> chatBoxItems;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private TMP_InputField _textInputField;

        private Queue<ChatBoxItem> chatItems = new(); // 재사용 큐
        private List<ChatBoxItem> activeItems = new(); // 순서 추적용 리스트
        private const int MaxChatCount = 30;
        private Query _chatRef;

        private void Start()
        {
            // 초기 채팅 박스 풀 만들기
            foreach (var item in chatBoxItems)
            {
                item.gameObject.SetActive(false);
                chatItems.Enqueue(item);
            }

            _chatRef = _firebaseManager.Database
                .GetReference("globalChat")
                .OrderByChild("timestamp")
                .LimitToLast(MaxChatCount);

            _chatRef.ChildAdded += OnNewMessage;
        }

        private void OnDisable()
        {
            _chatRef.ChildAdded -= OnNewMessage;
        }


        public void OnClickSendButton()
        {
            if (String.IsNullOrEmpty(_textInputField.text))
            {
                Debug.LogWarning("메시지가 비어있습니다");
                return;
            }
            else if (_textInputField.text.Length > 200)
            {
                Debug.LogWarning("메시지가 너무 깁니다");
                return;
            }

            string message = _textInputField.text;
            string sender = _firebaseManager.Auth.CurrentUser.DisplayName;

            ChatMessage newMessage = new ChatMessage(sender, message);

            _firebaseManager.Database
                .GetReference("globalChat")
                .Push()
                .SetRawJsonValueAsync(JsonUtility.ToJson(newMessage))
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log("메시지 전송 성공");
                    }
                    else
                    {
                        Debug.LogWarning($"메시지 전송 실패: {task.Exception}");
                    }
                });

            _textInputField.text = String.Empty;
        }


        private void OnNewMessage(object sender, ChildChangedEventArgs args)
        {
            if (args.Snapshot == null || !args.Snapshot.Exists) return;

            ChatMessage msg = JsonUtility.FromJson<ChatMessage>(args.Snapshot.GetRawJsonValue());
            AddMessageToUI(msg.sender, msg.text);
        }

        private void AddMessageToUI(string sender, string message)
        {
            // 초과된 메시지는 재활용
            if (activeItems.Count >= MaxChatCount)
            {
                ChatBoxItem oldItem = activeItems[0];
                activeItems.RemoveAt(0);
                oldItem.gameObject.SetActive(false);
                chatItems.Enqueue(oldItem);
            }

            if (chatItems.Count == 0)
            {
                Debug.LogWarning("ChatBoxItem 큐에 여유가 없습니다.");
                return;
            }

            ChatBoxItem item = chatItems.Dequeue();
            item.Init(sender, message);
            item.transform.SetAsLastSibling(); // 맨 아래로
            item.gameObject.SetActive(true);

            activeItems.Add(item);

            // 스크롤 맨 아래로 내리기
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
