using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIN
{
    public interface IGameManager
    {
        public void GoToRoom();
        public void CalculateResult(List<MatchPlayerData> result);
    }
}
