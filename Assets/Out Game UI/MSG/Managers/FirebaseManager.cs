using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class FirebaseManager : MonoBehaviour, IFirebaseManager
    {
        #region Fields And Properties

        private FirebaseApp _app;
        private FirebaseAuth _auth;
        private FirebaseDatabase _database;


        public FirebaseApp App
        {
            get
            {
                if (_app == null)
                {
                    _app = FirebaseApp.DefaultInstance;
                }
                return _app;
            }
        }

        public FirebaseAuth Auth
        {
            get
            {
                if (_auth == null)
                {
                    _auth = FirebaseAuth.DefaultInstance;
                }
                return _auth;
            }
        }

        public FirebaseDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = FirebaseDatabase.DefaultInstance;
                }
                return _database;
            }
        }
        #endregion

        private void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                Firebase.DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Debug.Log("파이어 베이스 설정이 모두 충족되어 사용할 수 있는 상황");
                    _app = FirebaseApp.DefaultInstance;
                    _auth = FirebaseAuth.DefaultInstance;
                    _database = FirebaseDatabase.DefaultInstance;

                    _database.GoOnline();
                }
                else
                {
                    Debug.LogError($"파이어 베이스 설정이 충족되지 않아 실패했습니다. 이유: {dependencyStatus}");
                    _app = null;
                    _auth = null;
                    _database = null;
                }
            });
        }
    }
}

