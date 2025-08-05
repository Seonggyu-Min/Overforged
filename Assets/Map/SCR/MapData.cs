using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public string Name;
    public MapTheme mapTheme;
    public Sprite Image;
    public List<Transform> SpawnPoints;
    public GameObject MapPrefabs;

    public enum MapTheme
    {
        Village,
        Beach,
        Desert,
        Forest,
        Volcano
    }
}
