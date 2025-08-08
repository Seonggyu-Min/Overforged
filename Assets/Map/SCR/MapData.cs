using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public string Name;
    public MapTheme mapTheme;
    public Sprite Image;
    public List<Transform> SpawnPoints;
    public List<TeamSmithingTools> teamSmithingTools;
    public GameObject MapPrefabs;

    public enum MapTheme
    {
        Village,
        Beach,
        Desert,
        Forest,
        Volcano
    }

    public void SetTeam(List<int> teamNum)
    {
        for (int i = 0; i < teamSmithingTools.Count; i++)
        {
            teamSmithingTools[i].SetTeam(teamNum[i]);
        }
    }
}
