using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public interface IFirebaseManager
    {
        FirebaseApp App { get; }
        FirebaseAuth Auth { get; }
        FirebaseDatabase Database { get; }
    }
}

