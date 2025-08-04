using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class InGameStatusUpdater : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;

        [SerializeField] private float _sendInterval = 10f;
        private Coroutine _SendOnlineCO;

        private void OnEnable()
        {
            _SendOnlineCO = StartCoroutine(SendOnlineStatusRoutine());
        }

        private void OnDisable()
        {
            if (_SendOnlineCO != null)
                StopCoroutine(_SendOnlineCO);
        }

        private IEnumerator SendOnlineStatusRoutine()
        {
            while (true)
            {
                UpdateLastSeen();
                yield return new WaitForSeconds(_sendInterval);
            }
        }

        private void UpdateLastSeen()
        {
            string uid = _firebaseManager.Auth.CurrentUser.UserId;
            DatabaseReference statusRef = _firebaseManager.Database.GetReference("lobbyStatus").Child(uid);

            Dictionary<string, object> updateData = new()
            {
                ["lastSeen"] = ServerValue.Timestamp,
                ["isGaming"] = true
            };

            statusRef.UpdateChildrenAsync(updateData);
        }
    }
}
