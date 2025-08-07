using System.Collections;
using System.Collections.Generic;
using SHG;
using UnityEngine;

public class TeamSmithingTools : MonoBehaviour
{
    public int TeamNum;
    [SerializeField] private List<SmithingToolComponent> smithingTools;

    public void SetTeam(int team)
    {
        TeamNum = team;
        foreach (var s in smithingTools)
            s.PlayerNetworkId = TeamNum;
    }
}
