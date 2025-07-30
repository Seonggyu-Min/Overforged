using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class LogInDebugText : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;
        [SerializeField] private TMP_Text _debugText;

        private void Update()
        {
            if (_firebaseManager.Auth.CurrentUser != null)
            {
                _debugText.text = $"로그인됨: {_firebaseManager.Auth.CurrentUser.Email}";
            }
            else
            {
                _debugText.text = "로그인되지 않음";
            }
        }
    }
}
