using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCR
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {
        public List<Character> characters;
    }

    [Serializable]
    public struct Character
    {
        public string name;
        public float walkSpeed;
        public float dashForce;
        public float workSpeed;
        public Material color;
    }
}

