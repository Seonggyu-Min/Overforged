using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public interface IScoreManager
    {
        public void AddScore(Player player ,int score);

    }
}
