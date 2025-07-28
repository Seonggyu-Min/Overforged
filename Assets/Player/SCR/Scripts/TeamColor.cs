using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SCR
{
    [CreateAssetMenu(fileName = "Team Color", menuName = "Scriptable Object/Team Color", order = int.MaxValue)]
    public class TeamColor : ScriptableObject
    {
        public List<Color> Color;
    }
}

