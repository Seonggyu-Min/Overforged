using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MIN
{
    public class GameQuitManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            TrySignOut();
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitEditorQuitListener()
        {
            EditorApplication.quitting += () =>
            {
                TrySignOut();
            };
        }
#endif

        private static void TrySignOut()
        {
            if (FirebaseAuth.DefaultInstance != null &&
                FirebaseAuth.DefaultInstance.CurrentUser != null)
            {
                FirebaseAuth.DefaultInstance.SignOut();
            }
        }
    }
}
