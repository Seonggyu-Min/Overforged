using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    [CreateAssetMenu(fileName = "AccountSO", menuName = "ScriptableObjects/AccountSO")]
    public class AccountSO : ScriptableObject
    {
        public string Email1;
        public string Password1;
        public string Email2;
        public string Password2;
        public string Email3;
        public string Password3;
    }
}
