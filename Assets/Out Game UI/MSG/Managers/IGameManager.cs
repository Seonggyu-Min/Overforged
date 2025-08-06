using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIN
{
    public interface IGameManager
    {
        public void SetGameEnd();
        public CalculatedTeam CalculateResult();

        public void SaveTeamResult();
        public bool IsTieForWinTeam();
    }
}
